using UnityEngine;

public abstract class ValueNode<T> : ValueNodeBase<T>
{
    [Output(ShowBackingValue.Always)]
    [SerializeField]
    protected T m_value;

    protected override T Value => m_value;
}
