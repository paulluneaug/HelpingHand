using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Singletons;

public class GraphManager : MonoBehaviourSingleton<GraphManager>
{
    [SerializeField]
    private SimpleGraph[] m_graphs;

    private Queue<SimpleGraph> m_graphQueue;
    private GraphRunner m_currentGraphRunner;

    public override void Initialize()
    {
        base.Initialize();
        m_graphQueue = new(m_graphs);
    }

    [Button("Start level")]
    private void StartLevel()
    {
        m_currentGraphRunner = StartGraph(m_graphQueue.Dequeue());
    }

    public GraphRunner StartGraph(SimpleGraph graph)
    {
        GraphRunner graphRunner = new GameObject($"GraphRunner [{graph.name}]").AddComponent<GraphRunner>();
        graphRunner.Initialize(graph);
        graphRunner.OnGraphEnded += OnGraphEnded;
        graphRunner.OnGraphPaused += OnGraphPaused;
        graphRunner.OnGraphResumed += OnGraphResumed;
        graphRunner.RunGraph();
        return graphRunner;
    }

    private void OnGraphEnded()
    {
        if (m_graphQueue.TryDequeue(out SimpleGraph graph))
        {
            m_currentGraphRunner = StartGraph(graph);
        }
    }

    private void OnGraphPaused()
    {
    }

    private void OnGraphResumed()
    {
        
    }
}
