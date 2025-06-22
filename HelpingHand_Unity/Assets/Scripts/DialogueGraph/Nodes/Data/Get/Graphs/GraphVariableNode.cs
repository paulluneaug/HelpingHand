using Sirenix.OdinInspector;

using UnityEngine;

public abstract class GraphVariableNode<T> : ValueNodeBase<GraphVariable<T>>
{
    [Output(ShowBackingValue.Never)]
    [SerializeField]
    protected GraphVariable<T> m_value;

    [SerializeField]
    protected T m_startValue;

    [ShowInInspector]
    [ReadOnly]
    private T CurrentValue => m_value.Value;

    protected override GraphVariable<T> Value => m_value;

    public override void Initialize()
    {
        base.Initialize();
        m_value = new GraphVariable<T>(m_startValue);
    }
}
