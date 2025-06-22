using System;
using System.Collections.Generic;
using System.IO.Ports;

using Sirenix.OdinInspector;

using UnityEngine;

using static CommandHeadersGlossary;

[Serializable]
public class ArduinoConnectorManager
{
    private const int QUEUE_CAPACITY = 128;

    [SerializeField] private bool m_enableArduinoConnection;

    [SerializeField] private ArduinoConnector m_arduinoConnector;

    [NonSerialized] private Queue<byte> m_recievedDatas;


    public void Initialize()
    {
        if (!m_enableArduinoConnection)
        {
            return;
        }

        m_recievedDatas = new Queue<byte>(QUEUE_CAPACITY);

        m_arduinoConnector.Init();

        m_arduinoConnector.OnMessageRecieved += OnArduinoMessageRecieved;
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

        int sequenceLength = simonSequence.Sequence.Length;

        if (sequenceLength == 0)
        {
            return;
        }

        int dataOffset = 2; // Header + size

        int bufferSize = dataOffset + sequenceLength / 4 + sequenceLength % 4 != 0 ? 1 : 0;

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

    private void OnArduinoMessageRecieved(byte[] buffer, int recievedBytesCount)
    {
        for (int i = 0; i < recievedBytesCount; i++)
        {
            m_recievedDatas.Enqueue(buffer[i]);
        }


        bool recievedEnoughDatas = true;
        while (m_recievedDatas.Count > 0 && recievedEnoughDatas)
        {
            byte queueHead = m_recievedDatas.Peek();
            switch (queueHead)
            {
                case ERROR_HEADER:
                    recievedEnoughDatas &= TryProcessErrorDatas(m_recievedDatas);
                    break;

                default: // The header is discarded if unknown
                    Debug.LogError($"Unknown Header ({queueHead}) Next commands might not be working properly");
                    _ = m_recievedDatas.Dequeue();
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
