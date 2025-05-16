using System;
using System.Threading;

public class GraphRunnerHandler : IDisposable
{
    public CancellationToken StopToken => m_stopCancellation.Token;
    public CancellationTokenSource StopSource => m_stopCancellation;
    public CancellationToken PauseToken => m_pauseCancellation.Token;
    public CancellationToken ResumeToken => m_resumeCancellation.Token;
    public GraphRunner GraphRunner => m_graphRunner;
    public BaseNode CurrentNode { get; set; }
    public int Priority { get; set; }
    public bool IsRunning => m_isRunning;
    public bool IsPaused => m_isPaused;

    private readonly GraphRunner m_graphRunner;
    private CancellationTokenSource m_stopCancellation = new();
    private CancellationTokenSource m_pauseCancellation = new();
    private CancellationTokenSource m_resumeCancellation = new();
    private bool m_isRunning;
    private bool m_isPaused;

    public GraphRunnerHandler(GraphRunner graphRunner)
    {
        m_graphRunner = graphRunner;
    }
    
    public void Start()
    {
        m_isRunning = true;
        m_stopCancellation.Dispose();
        m_stopCancellation = new CancellationTokenSource();
    }

    public void Pause()
    {
        m_isPaused = true;
        m_resumeCancellation?.Dispose();
        m_resumeCancellation = new CancellationTokenSource();
        m_pauseCancellation?.Cancel();
        m_stopCancellation?.Cancel();
    }

    public void Resume()
    {
        m_isPaused = false;
        m_stopCancellation?.Dispose();
        m_stopCancellation = new CancellationTokenSource();
        m_pauseCancellation?.Dispose();
        m_pauseCancellation = new CancellationTokenSource();
        m_resumeCancellation?.Cancel();
    }

    public void Stop()
    {
        if (m_isPaused)
        {
            Resume();
        }
        m_isRunning = false;
        m_isPaused = false;
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