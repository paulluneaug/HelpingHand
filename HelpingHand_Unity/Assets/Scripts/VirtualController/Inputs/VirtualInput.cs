using System;

using UnityEngine;

public abstract class VirtualInput<T> : MonoBehaviour, IVirtualInput
{
    public T Value => m_value;
    public event Action<T> OnValueChanged;

    [SerializeField] private BaseVariable<T> m_linkedVariable;

    [NonSerialized] private T m_value;

    protected void ChangeValue(T newValue)
    {
        m_value = newValue;

        OnValueChanged?.Invoke(newValue);

        if (m_linkedVariable != null)
        {
            m_linkedVariable.Value = newValue;
        }
    }
}
