using UnityEngine;

using XNode;
using XNode.Odin;

public class ValueFloatNode : SerializableNode
{
    [Output(ShowBackingValue.Always)] [SerializeField]
    private float m_value;
    
    public override object GetValue(NodePort port)
    {
        return m_value;
    }
}