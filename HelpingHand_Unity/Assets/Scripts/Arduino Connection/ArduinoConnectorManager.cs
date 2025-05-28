using System;
using System.Collections.Generic;
using System.IO.Ports;

using UnityEngine;

using UnityUtility.CustomAttributes;

using static CommandHeadersGlossary;

public class ArduinoConnectorManager : MonoBehaviour
{
    private const int QUEUE_CAPACITY = 128;


    [Button(nameof(GetAvailablePorts))]
    [SerializeField] private ArduinoConnector m_arduinoConnector;

    [NonSerialized] private Queue<byte> m_recievedDatas;


    private void Start()
    {
        m_recievedDatas = new Queue<byte>(QUEUE_CAPACITY);

        m_arduinoConnector.Init();

        m_arduinoConnector.OnMessageRecieved += OnArduinoMessageRecieved;
    }

    private void OnDestroy()
    {
        m_arduinoConnector.OnMessageRecieved -= OnArduinoMessageRecieved;
        m_arduinoConnector.Close();
    }

    public void SendIndicatorState(int indicatorState)
    {
        Span<byte> messageBuffer = stackalloc byte[5];
        byte writeIndex = 0;

        messageBuffer[writeIndex++] = INDICATOR_STATE_HEADER;
        messageBuffer[writeIndex++] = (byte)((indicatorState >> 0) & 0xFF);
        messageBuffer[writeIndex++] = (byte)((indicatorState >> 8) & 0xFF);
        messageBuffer[writeIndex++] = (byte)((indicatorState >> 16) & 0xFF);
        messageBuffer[writeIndex++] = (byte)((indicatorState >> 24) & 0xFF);

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

    private void GetAvailablePorts()
    {
        Debug.Log("Available Ports :");
        foreach (string portName in SerialPort.GetPortNames())
        {
            Debug.Log($"- {portName}");
        }
    }

}
