using UnityEngine;

using XNode;
using XNode.Odin;

public class ValueBoolNode : SerializableNode
{
    [Output(ShowBackingValue.Always)] [SerializeField]
    private bool m_value;
    
    public override object GetValue(NodePort port)
    {
        return m_value;
    }
}