using UnityEngine;

using XNode;
using XNode.Odin;

public abstract class ValueNode<T> : SerializableNode
{
    [Output(ShowBackingValue.Always)] [SerializeField]
    private T m_value;
    
    public override object GetValue(NodePort port)
    {
        return m_value;
    }
}