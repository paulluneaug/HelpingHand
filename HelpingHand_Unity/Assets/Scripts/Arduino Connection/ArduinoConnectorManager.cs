using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Security.Policy;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.Extensions;

using static CommandHeadersGlossary;

[Serializable]
public class ArduinoConnectorManager
{
    [Serializable]
    private class ControllerSettings
    {
        public string SerialPort;
    }

    public bool IsReady => !m_enableArduinoConnection || m_ready;

    [SerializeField] private bool m_enableArduinoConnection;
    [SerializeField] private string m_controllerSettingsJsonPath;

    [NonSerialized] private bool m_ready = false;

    [NonSerialized] private ArduinoConnector m_arduinoConnector;


    public void Initialize()
    {

        if (!m_enableArduinoConnection)
        {
            return;
        }
        m_ready = false;

        string controllerSettingsJson = File.ReadAllText(Path.Combine(".", "ExternalAssets", m_controllerSettingsJsonPath));
        ControllerSettings settings = JsonUtility.FromJson<ControllerSettings>(controllerSettingsJson);

        m_arduinoConnector = new ArduinoConnector();
        m_arduinoConnector.OnSynAckRecieved += OnSynAckRecieved;
        m_arduinoConnector.Init(settings.SerialPort);

        SendAcks().Forget();
    }

    private async UniTask SendAcks()
    {
        await UniTask.WaitForSeconds(1.0f);
        m_arduinoConnector.SendAck();
    }

    private void OnSynAckRecieved()
    {
        m_arduinoConnector.OnSynAckRecieved -= OnSynAckRecieved;
        m_arduinoConnector.OnMessageRecieved += OnArduinoMessageRecieved;
        m_ready = true;
    }

    public void Dispose()
    {
        if (!m_enableArduinoConnection)
        {
            return;
        }

        m_arduinoConnector.OnMessageRecieved -= OnArduinoMessageRecieved;
        m_arduinoConnector.Close();
    }

    public void SendIndicatorState(int indicatorState)
    {
        if (!m_enableArduinoConnection)
        {
            return;
        }
        if (!m_ready)
        {
            Debug.LogError("Ardunio connection not ready");
            return;
        }

        Span<byte> messageBuffer = stackalloc byte[5];
        byte writeIndex = 0;

        messageBuffer[writeIndex++] = INDICATOR_STATE_HEADER;
        messageBuffer[writeIndex++] = (byte)((indicatorState >> 0) & 0xFF);
        messageBuffer[writeIndex++] = (byte)((indicatorState >> 8) & 0xFF);
        messageBuffer[writeIndex++] = (byte)((indicatorState >> 16) & 0xFF);
        messageBuffer[writeIndex++] = (byte)((indicatorState >> 24) & 0xFF);

        m_arduinoConnector.Send(messageBuffer);
    }

    public void SendSimonSequence(SimonSequence simonSequence)
    {
        if (!m_enableArduinoConnection)
        {
            return;
        }
        if (!m_ready)
        {
            Debug.LogError("Ardunio connection not ready");
            return;
        }

        int sequenceLength = simonSequence.Sequence.Length;

        if (sequenceLength == 0)
        {
            return;
        }

        int dataOffset = 2; // Header + size

        int bufferSize = dataOffset + sequenceLength / 4 + ((sequenceLength % 4 != 0) ? 1 : 0);

        Span<byte> messageBuffer = stackalloc byte[bufferSize];
        messageBuffer.Fill(0);

        messageBuffer[0] = SIMON_SEQUENCE_HEADER;
        messageBuffer[1] = (byte)sequenceLength;

        for (int sequenceIndex = 0; sequenceIndex < sequenceLength; sequenceIndex++)
        {
            int byteIndex = dataOffset + sequenceIndex / 4;
            int byteOffset = (sequenceIndex % 4) * 2;
            messageBuffer[byteIndex] |= (byte)((byte)simonSequence.Sequence[sequenceIndex] << byteOffset);
        }

        m_arduinoConnector.Send(messageBuffer);
    }

    [Button]
    private void FaderUp()
    {
        SendFaderPosition(true);
    }

    [Button]
    private void FaderDown()
    {
        SendFaderPosition(false);
    }


    public void SendFaderPosition(bool high)
    {
        if (!m_enableArduinoConnection)
        {
            return;
        }
        Span<byte> messageBuffer = stackalloc byte[2];
        byte writeIndex = 0;

        messageBuffer[writeIndex++] = FADER_POSITION_HEADER;
        messageBuffer[writeIndex++] = (byte)(high ? 1 : 0);

        m_arduinoConnector.Send(messageBuffer);
    }

    private void OnArduinoMessageRecieved(byte[] buffer, int recievedBytesCount)
    {
        bool recievedEnoughDatas = true;
        while (m_arduinoConnector.RecievedDatas.Count > 0 && recievedEnoughDatas)
        {
            byte queueHead = m_arduinoConnector.RecievedDatas.Peek();
            switch (queueHead)
            {
                case ERROR_HEADER:
                    recievedEnoughDatas &= TryProcessErrorDatas(m_arduinoConnector.RecievedDatas);
                    break;

                default: // The header is discarded if unknown
                    Debug.LogError($"Unknown Header ({queueHead}) Next commands might not be working properly");
                    _ = m_arduinoConnector.RecievedDatas.Dequeue();
                    break;
            }
        }
    }

    private bool TryProcessErrorDatas(Queue<byte> recievedDatas)
    {
        if (recievedDatas.Count < 2)
        {
            return false; // Should wait for more datas to arrive
        }

        _ = recievedDatas.Dequeue(); // Dequeue the header

        byte errorCode = recievedDatas.Dequeue();

        if (errorCode != 0)
        {
            Debug.LogError($"ArduinoError recieved : {errorCode}");
        }
        return true; // The command was processed and removed from the queue
    }

    [Button]
    private void GetAvailablePorts()
    {
        Debug.Log("Available Ports :");
        foreach (string portName in SerialPort.GetPortNames())
        {
            Debug.Log($"- {portName}");
        }
    }

}
