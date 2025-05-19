using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Singletons;

using Utils;

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

    [SerializeField] 
    private float m_delayBetweenInterruptions = 0.5f;

    private Queue<SimpleGraph> m_graphQueue;

    private readonly PriorityQueue<GraphRunnerHandler, (int priority, int depth)> m_interruptionQueue =
        new(Comparer<(int priority, int depth)>.Create((i1, i2) =>
        {
            int comparison = -i1.priority.CompareTo(i2.priority);
            if (comparison == 0)
            {
                comparison = -i1.depth.CompareTo(i2.depth);
            }

            return comparison;
        }));

    private readonly Dictionary<GraphRunnerHandler, (bool returned, bool passed)> m_interruptionDictionary = new();

    private GraphRunner m_currentGraphRunner;
    private Dictionary<SimpleGraph, GraphRunner> m_graphDictionary = new();
    private float m_timeWhenCheckingInterruption;

    public override void Initialize()
    {
        base.Initialize();
        m_graphQueue = new(m_mainSequence);
        m_interruptionQueue.Clear();
        m_interruptionDictionary.Clear();
    }

    [Button("Start sequence")]
    private void StartSequence()
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


    public async UniTask<bool> Interrupt(int interruptionDepth, GraphRunnerHandler handler)
    {
        lock (m_interruptionQueue)
        {
            // Test if this graph can interrupt the running graph
            if (m_currentGraphRunner != null && handler.Priority <= m_currentGraphRunner.Handler.Priority)
            {
                Debug.Log($"{Time.frameCount} Graph ({handler.GraphRunner.name}) cannot interrupt. Priority={handler.Priority} currentGraph's priority={m_currentGraphRunner.Handler.Priority}");
                return false;
            }

            m_interruptionQueue.Enqueue(handler, (handler.Priority, interruptionDepth));
        }

        m_interruptionDictionary[handler] = (false, false);

        await UniTask.WaitUntil(() => m_interruptionDictionary[handler].returned);
        return m_interruptionDictionary[handler].passed;
    }

    private void Update()
    {
        // Check every n seconds
        if (Time.time > m_timeWhenCheckingInterruption)
        {
            m_timeWhenCheckingInterruption = Time.time + m_delayBetweenInterruptions;
            lock (m_interruptionQueue)
            {
                DequeueInterrupting();
            }
        }
    }

    private void DequeueInterrupting()
    {
        if (m_interruptionQueue.Count == 0)
        {
            return;
        }

        // Take the graph with most priority and make it the interrupting graph
        GraphRunnerHandler interruptingGraph = m_interruptionQueue.Dequeue();

        // Tell the others to cancel their interruption
        foreach (var item in m_interruptionQueue.UnorderedItems)
        {
            Debug.Log($"Graph ({item.Element.GraphRunner.name}) cannot interrupt. Another graph has been chosen.");
            m_interruptionDictionary[item.Element] = (true, false);
        }

        m_interruptionQueue.Clear();

        // Interrupt the current graph and mark it to resume when the interrupting graph is finished
        if (m_currentGraphRunner != null)
        {
            m_currentGraphRunner.PauseGraph();
            GraphRunner interruptedGraph = m_currentGraphRunner;
            interruptingGraph.GraphRunner.OnGraphStopped += () =>
            {
                m_currentGraphRunner = interruptedGraph;
                m_currentGraphRunner.ResumeGraph();
            };
        }

        m_currentGraphRunner = interruptingGraph.GraphRunner;
        
        // Tell the interrupting graph to continue
        m_interruptionDictionary[interruptingGraph] = (true, true);
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