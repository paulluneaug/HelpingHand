using System;
using System.Linq;

using UnityEngine;

public abstract class VirtualInput<T> : MonoBehaviour, IVirtualInput
{
    public T Value => m_value;
    public event Action<T> OnValueChanged;

    protected abstract BaseVariable<T> LinkedVariable { get; }

    [NonSerialized] private T m_value;

    protected void SetValue(T newValue)
    {
        m_value = newValue;

        if (LinkedVariable != null)
        {
            LinkedVariable.Value = newValue;
        }

        OnValueChanged?.Invoke(newValue);
    }

    protected virtual void SetValueWithoutNotify(T newValue)
    {
        m_value = newValue;

        if (LinkedVariable != null)
        {
            LinkedVariable.SetValueWithoutNotify(newValue);
        }
    }
}
