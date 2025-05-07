using System;

using Sirenix.OdinInspector;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class GraphRunner : MonoBehaviour
{
    public event Action OnGraphStarted;
    public event Action OnGraphEnded;
    public event Action OnGraphCancelled;
    public event Action OnGraphStopped;
    public event Action OnGraphPaused;
    public event Action OnGraphResumed;

    private GraphRunnerHandler m_graphRunnerHandler;
    private SimpleGraph m_graph;

    private void Awake()
    {
        m_graphRunnerHandler = new GraphRunnerHandler();
    }

    public void Initialize(SimpleGraph graph)
    {
        m_graph = graph;
        m_graph.Initialize();
    }

    [Button("Reset")]
    [ButtonGroup("Controls")]
    public void ResetGraph()
    {
        m_graph.Initialize();
    }

    [Button("Start")]
    [ButtonGroup("Controls")]
    public void RunGraph()
    {
        Debug.Log($"Graph [{m_graph.name}]: Start");
        RunGraph(m_graph);
    }

    public void RunGraph(SimpleGraph graph)
    {
        m_graphRunnerHandler.Start();
        RunGraphAsync(graph).Forget();
    }

    [Button("Stop")]
    [ButtonGroup("Controls")]
    public void StopGraph()
    {
        m_graphRunnerHandler.Stop();
    }

    [Button("Pause")]
    [ButtonGroup("Controls")]
    public void PauseGraph()
    {
        m_graphRunnerHandler.Pause();
        OnGraphPaused?.Invoke();
    }

    [Button("Resume")]
    [ButtonGroup("Controls")]
    public void ResumeGraph()
    {
        m_graphRunnerHandler.Resume();
        OnGraphResumed?.Invoke();
    }

    private async UniTask RunGraphAsync(SimpleGraph graph)
    {
        OnGraphStarted?.Invoke();
        bool isCanceled = await graph.Run(m_graphRunnerHandler).SuppressCancellationThrow();
        if (isCanceled)
        {
            Debug.Log($"Graph [{m_graph.name}]: was stopped prematurely");
            OnGraphCancelled?.Invoke();
        }
        else
        {
            Debug.Log($"Graph [{m_graph.name}]: ended");
            OnGraphEnded?.Invoke();
        }

        OnGraphStopped?.Invoke();
    }

    private void OnDestroy()
    {
        m_graphRunnerHandler.Dispose();
        OnGraphStarted = null;
        OnGraphEnded = null;
        OnGraphCancelled = null;
        OnGraphStopped = null;
        OnGraphPaused = null;
        OnGraphResumed = null;
    }
}