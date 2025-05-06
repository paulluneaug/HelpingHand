using System;
using System.Threading;

public class GraphRunnerHandler : IDisposable
{
    public CancellationToken StopToken => m_stopCancellation.Token;
    public CancellationToken PauseToken => m_pauseCancellation.Token;
    public CancellationToken ResumeToken => m_resumeCancellation.Token;

    private CancellationTokenSource m_stopCancellation = new();
    private CancellationTokenSource m_pauseCancellation = new();
    private CancellationTokenSource m_resumeCancellation = new();

    public void Start()
    {
        m_stopCancellation?.Dispose();
        m_stopCancellation = new CancellationTokenSource();
    }

    public void Pause()
    {
        m_resumeCancellation?.Dispose();
        m_resumeCancellation = new CancellationTokenSource();
        m_pauseCancellation?.Cancel();
        m_stopCancellation?.Cancel();
    }

    public void Resume()
    {
        m_stopCancellation?.Dispose();
        m_stopCancellation = new CancellationTokenSource();
        m_pauseCancellation?.Dispose();
        m_pauseCancellation = new CancellationTokenSource();
        m_resumeCancellation?.Cancel();
    }

    public void Stop()
    {
        m_stopCancellation.Cancel();
    }

    public void Dispose()
    {
        m_stopCancellation.Cancel();
        m_pauseCancellation.Cancel();
        m_resumeCancellation.Cancel();
        m_stopCancellation.Dispose();
        m_pauseCancellation.Dispose();
        m_resumeCancellation.Dispose();
    }
}