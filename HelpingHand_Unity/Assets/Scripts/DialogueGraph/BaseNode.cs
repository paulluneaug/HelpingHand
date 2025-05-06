using System.Collections;

using XNode;
using XNode.Odin;

public abstract class BaseNode : SerializableNode
{
    public virtual void Initialize() { }

    public virtual IEnumerator Execute() { yield break; }

    protected IEnumerator ContinueFlow()
    {
        NodePort outputPort = GetOutputPort("m_out");
        if (outputPort.ConnectionCount > 1)
        {
            foreach (NodePort otherPort in outputPort.GetConnections())
            {
                BaseNode nextNode = otherPort.node as BaseNode;
                if (nextNode != null)
                {
                    // TODO trouver un moyen de lancer en parallèle et qu'on puisse attendre le tout (UniTask ?)
                    GraphRunner.Instance.StartCoroutine(nextNode.Execute());
                }
            }
        }
        else
        {
            NodePort otherPort = outputPort.Connection;
            BaseNode nextNode = otherPort.node as BaseNode;
            if (nextNode != null)
            {
                yield return nextNode.Execute();
            }
        }
    }
}
