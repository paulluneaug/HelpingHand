using System;
using System.Collections;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Singletons;

public class GraphRunner : MonoBehaviourSingleton<GraphRunner>
{
    [SerializeField]
    private SimpleGraph m_graph;

    protected override void Start()
    {
        m_graph.Initialize();
    }

    [Button("Run")]
    public void RunGraph()
    {
        Debug.Log($"Graph [{m_graph.name}]: Start");
        RunGraph(m_graph);
    }

    public void RunGraph(SimpleGraph graph)
    {
        StartCoroutine(RunGraphCoroutine(graph));
    }

    private IEnumerator RunGraphCoroutine(SimpleGraph graph)
    {
        yield return graph.Run();
        Debug.Log($"Graph [{m_graph.name}]: End");
    }
}
