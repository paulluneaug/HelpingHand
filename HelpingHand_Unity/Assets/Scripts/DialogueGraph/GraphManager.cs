using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Singletons;

public class GraphManager : MonoBehaviourSingleton<GraphManager>
{
    public bool CurrentNodeCanBeInterrupted
    {
        get
        {
            if (m_currentGraphRunner != null && m_currentGraphRunner.Handler.CurrentNode is InterruptableNode interruptableNode)
            {
                return interruptableNode.Interruptable;
            }
            else
            {
                return true;
            }
        }
    }

    public GraphRunner CurrentGraphRunner => m_currentGraphRunner;

    [SerializeField]
    private SimpleGraph[] m_mainSequence;

    [SerializeField]
    private SimpleGraph[] m_parallelExecution;

    private Queue<SimpleGraph> m_graphQueue;
    private GraphRunner m_currentGraphRunner;
    private readonly Dictionary<SimpleGraph, GraphRunner> m_graphDictionary = new();

    public override void Initialize()
    {
        base.Initialize();
        m_graphQueue = new(m_mainSequence);
    }

    [Button("Start level")]
    private void StartLevel()
    {
        StartLevelAsync().Forget();
    }

    private async UniTaskVoid StartLevelAsync()
    {
        StartMainSequenceGraph(m_graphQueue.Dequeue()).Forget();
        foreach (SimpleGraph graph in m_parallelExecution)
        {
            GraphRunner graphRunner = await CreateGraphRunner(graph);
            graphRunner.StartGraph();
        }
    }

    private async UniTaskVoid StartMainSequenceGraph(SimpleGraph graph)
    {
        GraphRunner graphRunner = await CreateGraphRunner(graph);
        graphRunner.OnGraphEnded += OnGraphEnded;
        graphRunner.OnGraphPaused += OnGraphPaused;
        graphRunner.OnGraphResumed += OnGraphResumed;
        graphRunner.StartGraph();
        m_currentGraphRunner = graphRunner;
    }

    public async UniTask<GraphRunner> CreateGraphRunner(SimpleGraph graph)
    {
        bool isRunning = false;
        // If graph is already running, wait for its completion
        if (m_graphDictionary.TryGetValue(graph, out GraphRunner existingRunner))
        {
            isRunning = true;
            existingRunner.OnGraphEnded += () => isRunning = false;
        }

        await UniTask.WaitUntil(() => isRunning == false);
        GraphRunner graphRunner = new GameObject($"GraphRunner [{graph.name}]").AddComponent<GraphRunner>();
        graphRunner.Initialize(graph);

        m_graphDictionary[graph] = graphRunner;
        return graphRunner;
    }

    public bool TryGetGraphRunner(SimpleGraph graph, out GraphRunner graphRunner)
    {
        return m_graphDictionary.TryGetValue(graph, out graphRunner);
    }

    private void OnGraphEnded()
    {
        m_currentGraphRunner = null;
        if (m_graphQueue.TryDequeue(out SimpleGraph graph))
        {
            StartMainSequenceGraph(graph).Forget();
        }
    }

    private void OnGraphPaused()
    {
    }

    private void OnGraphResumed()
    {

    }

    public void Interrupt(GraphRunnerHandler handler)
    {
        m_currentGraphRunner?.PauseGraph();
        GraphRunner interruptedGraph = m_currentGraphRunner;
        handler.GraphRunner.OnGraphEnded += () =>
        {
            interruptedGraph?.ResumeGraph();
        };
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        foreach (GraphRunner graphRunner in m_graphDictionary.Values)
        {
            graphRunner.OnApplicationQuit();
        }
    }
#endif
}
