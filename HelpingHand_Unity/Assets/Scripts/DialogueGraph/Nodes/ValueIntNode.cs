using UnityEngine;

using XNode;
using XNode.Odin;

public class ValueIntNode : SerializableNode
{
    [Output(ShowBackingValue.Always)] [SerializeField]
    private int m_value;
    
    public override object GetValue(NodePort port)
    {
        return m_value;
    }
}