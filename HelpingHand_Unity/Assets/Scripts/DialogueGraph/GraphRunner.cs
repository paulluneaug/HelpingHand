using Sirenix.OdinInspector;

using Cysharp.Threading.Tasks;

using UnityEngine;

using UnityUtility.Singletons;

public class GraphRunner : MonoBehaviourSingleton<GraphRunner>
{
    [SerializeField]
    private SimpleGraph m_graph;

    private GraphRunnerHandler m_graphRunnerHandler;
    
    protected override void Start()
    {
        m_graph.Initialize();
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
        RunGraphAsync(graph).Forget();
    }

    [Button("Stop")][ButtonGroup("Controls")]
    public void StopGraph()
    {
        m_graphRunnerHandler.Stop();
    }

    [Button("Pause")][ButtonGroup("Controls")]
    public void PauseGraph()
    {
        m_graphRunnerHandler.Pause();
    }

    [Button("Resume")][ButtonGroup("Controls")]
    public void ResumeGraph()
    {
        m_graphRunnerHandler.Resume();
    }

    private async UniTask RunGraphAsync(SimpleGraph graph)
    {
        bool isCanceled = await graph.Run(m_graphRunnerHandler).SuppressCancellationThrow();
        if (isCanceled)
        {
            Debug.Log($"Graph [{m_graph.name}]: was stopped prematurely");
        }
        else
        {
            Debug.Log($"Graph [{m_graph.name}]: ended");
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_graphRunnerHandler.Dispose();
    }
}
