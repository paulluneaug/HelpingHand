using System;
using System.Collections;
using System.Threading;

using Sirenix.OdinInspector;

using Cysharp.Threading.Tasks;

using UnityEngine;

using UnityUtility.Singletons;

public class GraphRunner : MonoBehaviourSingleton<GraphRunner>
{
    [SerializeField]
    private SimpleGraph m_graph;

    private GraphRunnerHandler m_graphRunnerHandler;
    private CancellationTokenSource m_stopGraphCancellation;
    private CancellationTokenSource m_pauseGraphCancellation;
    private CancellationTokenSource m_resumeGraphCancellation;
    
    protected override void Start()
    {
        m_graph.Initialize();
        m_stopGraphCancellation = new CancellationTokenSource();
        m_pauseGraphCancellation = new CancellationTokenSource();
        m_resumeGraphCancellation = new CancellationTokenSource();
        m_graphRunnerHandler = new GraphRunnerHandler();
    }

    [Button("Start")][ButtonGroup("Controls")]
    public void RunGraph()
    {
        Debug.Log($"Graph [{m_graph.name}]: Start");
        RunGraph(m_graph);
    }

    public void RunGraph(SimpleGraph graph)
    {
        m_graphRunnerHandler.Start();
        // m_stopGraphCancellation?.Dispose();
        // m_stopGraphCancellation = new CancellationTokenSource();
        RunGraphAsync(graph).Forget();
    }

    [Button("Stop")][ButtonGroup("Controls")]
    public void StopGraph()
    {
        // m_stopGraphCancellation.Cancel();
        m_graphRunnerHandler.Stop();
    }

    [Button("Pause")][ButtonGroup("Controls")]
    public void PauseGraph()
    {
        m_graphRunnerHandler.Pause();
        // m_pauseGraphCancellation?.Cancel();
        // m_resumeGraphCancellation?.Dispose();
        // m_resumeGraphCancellation = new CancellationTokenSource();
    }

    [Button("Resume")][ButtonGroup("Controls")]
    public void ResumeGraph()
    {
        m_graphRunnerHandler.Resume();
        // m_resumeGraphCancellation?.Cancel();
        // m_pauseGraphCancellation?.Dispose();
        // m_pauseGraphCancellation = new CancellationTokenSource();
    }

    private async UniTask RunGraphAsync(SimpleGraph graph)
    {
        // await graph.Run(m_stopGraphCancellation.Token, () => m_pauseGraphCancellation.Token, () => m_resumeGraphCancellation.Token);
        await graph.Run(m_graphRunnerHandler);
        Debug.Log($"Graph [{m_graph.name}]: End");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_graphRunnerHandler.Dispose();
        // m_stopGraphCancellation.Cancel();
        // m_stopGraphCancellation.Dispose();
        // m_pauseGraphCancellation.Cancel();
        // m_pauseGraphCancellation.Dispose();
        // m_resumeGraphCancellation.Cancel();
        // m_resumeGraphCancellation.Dispose();
    }
}
