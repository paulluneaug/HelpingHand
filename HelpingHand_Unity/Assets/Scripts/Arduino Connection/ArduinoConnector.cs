using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

public class ArduinoConnector
{
    private const int BUFFER_SIZE = 32;
    private const int BAUD = 9600;
    private const int QUEUE_CAPACITY = 128;

    public event Action<byte[], int> OnMessageRecieved;
    public event Action OnSynAckRecieved;

    public Queue<byte> RecievedDatas => m_recievedDatas;

    [NonSerialized] private SerialPort m_serialPort;
    [NonSerialized] private byte[] m_buffer;
    [NonSerialized] private bool m_open;
    [NonSerialized] private bool m_synackRecieved;

    [NonSerialized] private CancellationTokenSource m_awaitDataTaskCancellationTokenSource;
    [NonSerialized] private Task m_awaitDataTask;

    [NonSerialized] private Queue<byte> m_recievedDatas;


    public void Init(string portName)
    {
        m_buffer = new byte[BUFFER_SIZE];

        m_recievedDatas = new Queue<byte>(QUEUE_CAPACITY);

        m_serialPort = new SerialPort(portName, BAUD)
        {
            ReadTimeout = 50,

            RtsEnable = true,
            DtrEnable = true
        };

        try
        {
            m_serialPort.Open();
            m_open = true;

            m_awaitDataTaskCancellationTokenSource = new CancellationTokenSource();

            m_awaitDataTask = Task.Factory.StartNew(AwaitDatas, m_awaitDataTaskCancellationTokenSource.Token);
            m_synackRecieved = false;

        }
        catch (IOException io)
        {
            Debug.LogException(io);
        }
    }

    public void Close()
    {
        m_awaitDataTaskCancellationTokenSource?.Cancel();
        m_awaitDataTask?.Dispose();
        m_serialPort?.Close();
        m_serialPort = null;
        m_open = false;
        m_synackRecieved = false;
    }

    public void Send(Span<byte> buffer)
    {
        m_serialPort.BaseStream.Write(buffer);
        //m_serialPort.WriteLine(message);
        m_serialPort.BaseStream.Flush();
        Debug.Log($"Sent {buffer.Length} bytes");
    }

    public void SendAck()
    {
        if (!m_open)
        {
            return;
        }

        Span<byte> message = stackalloc byte[2];
        message[0] = CommandHeadersGlossary.ACK_HEADER;
        message[1] = 127;
        Send(message);
    }

    private bool TryProcessSynAck()
    {
        if (m_recievedDatas.Count < 2)
        {
            return false;
        }
        
        if (m_recievedDatas.Dequeue() == CommandHeadersGlossary.SYNACK_HEADER)
        {
            _ = m_recievedDatas.Dequeue();
            return true;
        }
        return false;
    }

    private async Task AwaitDatas()
    {
        while (m_open)
        {
            int readBytesCount;
            try
            {
                readBytesCount = m_serialPort.Read(m_buffer, 0, BUFFER_SIZE);
            }
            catch (TimeoutException)
            {
                readBytesCount = 0;
            }

            if (readBytesCount != 0)
            {
                Debug.Log($"Recieved {readBytesCount} bytes");

                for (int i = 0; i < readBytesCount; i++)
                {
                    m_recievedDatas.Enqueue(m_buffer[i]);
                }

                if (m_synackRecieved)
                {
                    OnMessageRecieved?.Invoke(m_buffer, readBytesCount);
                }
                else if (TryProcessSynAck())
                {
                    m_synackRecieved = true;
                    OnSynAckRecieved?.Invoke();
                    continue;
                }
            }
            await Task.Delay(10);
        }
    }
}

