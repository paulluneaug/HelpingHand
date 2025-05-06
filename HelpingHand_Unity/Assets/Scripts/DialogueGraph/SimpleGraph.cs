using System.Collections;

using UnityEngine;

using XNode;

[CreateAssetMenu]
public class SimpleGraph : NodeGraph
{
    private StartNode m_startNode;
    
    public void Initialize()
    {
        foreach (Node node in nodes)
        {
            if (node is BaseNode nodeBase)
            {
                nodeBase.Initialize();
            }

            if (node is StartNode startNode)
            {
                m_startNode = startNode;
            }
        }
    }

    public IEnumerator Run()
    {
        yield return m_startNode.Execute();
    }
}
