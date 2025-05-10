using System;
using System.Collections.Generic;

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
    private Dictionary<SimpleGraph, GraphRunner> m_graphDictionary = new();

    public override void Initialize()
    {
        base.Initialize();
        m_graphQueue = new(m_mainSequence);
    }

    [Button("Start level")]
    private void StartLevel()
    {
        m_currentGraphRunner = StartMainSequenceGraph(m_graphQueue.Dequeue());
        foreach (SimpleGraph graph in m_parallelExecution)
        {
            GraphRunner graphRunner = CreateGraphRunner(graph);
            graphRunner.StartGraph();
        }
    }

    private GraphRunner StartMainSequenceGraph(SimpleGraph graph)
    {
        GraphRunner graphRunner = CreateGraphRunner(graph);
        graphRunner.OnGraphEnded += OnGraphEnded;
        graphRunner.OnGraphPaused += OnGraphPaused;
        graphRunner.OnGraphResumed += OnGraphResumed;
        graphRunner.StartGraph();
        return graphRunner;
    }

    public GraphRunner CreateGraphRunner(SimpleGraph graph)
    {
        GraphRunner graphRunner = new GameObject($"GraphRunner [{graph.name}]").AddComponent<GraphRunner>();
        graphRunner.Initialize(graph);
        m_graphDictionary.Add(graph, graphRunner);
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
            m_currentGraphRunner = StartMainSequenceGraph(graph);
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
