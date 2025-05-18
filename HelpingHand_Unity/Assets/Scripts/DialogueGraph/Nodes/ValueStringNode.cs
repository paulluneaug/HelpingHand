using UnityEngine;

using XNode;
using XNode.Odin;

public class ValueStringNode : SerializableNode
{
    [Output(ShowBackingValue.Always)] [SerializeField]
    private string m_value;
    
    public override object GetValue(NodePort port)
    {
        return m_value;
    }
}
