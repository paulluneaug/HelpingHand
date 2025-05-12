using UnityEngine;

using XNode;
using XNode.Odin;

public class StringNode : SerializableNode
{
    [Output(ShowBackingValue.Always)] [SerializeField]
    private string m_stringOut;
    
    public override object GetValue(NodePort port)
    {
        return m_stringOut;
    }
}
