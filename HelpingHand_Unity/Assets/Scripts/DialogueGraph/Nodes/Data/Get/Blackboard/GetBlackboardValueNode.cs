using UnityEngine;

using XNode;

[NodeWidth(250)]
[NodeTint(0f, 0.4784314f, 0.6509804f)]
[CreateNodeMenu("Data/Get/Blackboard Value")]
public class GetBlackboardValueNode : BaseNode
{
    [Input(ShowBackingValue.Never)]
    private readonly object m_inValue;

    [Output(ShowBackingValue.Never)]
    [SerializeField]
    private object m_valueOut;

    [SerializeField]
    private string m_key;

    public override object GetValue(NodePort port)
    {
        if (GraphBlackboard.Instance.TryGetValue(m_key, out object value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }
}