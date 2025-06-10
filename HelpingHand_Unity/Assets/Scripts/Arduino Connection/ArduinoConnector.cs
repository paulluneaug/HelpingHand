using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

[Serializable]
public class ArduinoConnector
{
    private const int BUFFER_SIZE = 32;

    public event Action<byte[], int> OnMessageRecieved;

    [SerializeField] private string m_port;
    [SerializeField] private int m_baud;

    [NonSerialized] private SerialPort m_serialPort;
    [NonSerialized] private byte[] m_buffer;
    [NonSerialized] private bool m_open;

    [NonSerialized] private CancellationTokenSource m_awaitDataTaskCancellationTokenSource;
    [NonSerialized] private Task m_awaitDataTask;


    public void Init()
    {
        m_buffer = new byte[BUFFER_SIZE];

        m_serialPort = new SerialPort(m_port, m_baud)
        {
            ReadTimeout = 50,

            RtsEnable = true,
            DtrEnable = true
        };

        m_serialPort.Open();
        m_open = true;

        m_awaitDataTaskCancellationTokenSource = new CancellationTokenSource();

        m_awaitDataTask = Task.Factory.StartNew(AwaitDatas, m_awaitDataTaskCancellationTokenSource.Token);
    }

    public void Close()
    {
        m_awaitDataTaskCancellationTokenSource.Cancel();
        m_awaitDataTask.Dispose();
        m_serialPort.Close();
        m_serialPort = null;
        m_open = false;
    }

    public void Send(Span<byte> buffer)
    {
        m_serialPort.BaseStream.Write(buffer);
        //m_serialPort.WriteLine(message);
        m_serialPort.BaseStream.Flush();
        Debug.Log($"Sent {buffer.Length} bytes");
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
                OnMessageRecieved?.Invoke(m_buffer, readBytesCount);
            }
            await Task.Delay(10);
        }
    }
}

