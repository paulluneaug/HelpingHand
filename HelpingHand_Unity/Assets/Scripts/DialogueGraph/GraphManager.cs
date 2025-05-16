using System;
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

    [SerializeField] 
    private float m_delayBetweenInterruptions = 0.5f;

    private Queue<SimpleGraph> m_graphQueue; 
    private readonly SortedSet<GraphRunnerHandler> m_interruptionQueue = new(Comparer<GraphRunnerHandler>.Create((h1, h2) => h1.Priority.CompareTo(h2.Priority)));
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

    
    public async UniTask<bool> Interrupt(GraphRunnerHandler handler)
    {
        // Test if this graph can interrupt the running graph
        if (handler.Priority <= m_currentGraphRunner.Handler.Priority)
        {
            return false;
        }
        
        m_interruptionQueue.Add(handler);
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
            DequeueInterrupting();
        }
    }

    private void DequeueInterrupting()
    {
        if (m_interruptionQueue.Count == 0)
        {
            return;
        }
        
        // Take the graph with most priority and make it the interrupting graph
        GraphRunnerHandler interruptingGraph = m_interruptionQueue.Max;
        
        // Tell the others to cancel their interruption
        foreach (GraphRunnerHandler otherHandler in m_interruptionQueue)
        {
            m_interruptionDictionary[otherHandler] = (true, false);
        }
        m_interruptionQueue.Clear();
        
        // Interrupt the current graph and mark it to resume when the interrupting graph is finished
        m_currentGraphRunner?.PauseGraph();
        GraphRunner interruptedGraph = m_currentGraphRunner;
        interruptingGraph.GraphRunner.OnGraphEnded += () =>
        {
            interruptedGraph?.ResumeGraph();
        };
        
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
