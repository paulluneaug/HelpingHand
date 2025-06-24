using System;

using Events;

using Sirenix.OdinInspector;

using UnityEngine;

public interface IVariable
{

}

public abstract class BaseVariable : BaseGameEvent, IVariable
{

}

public abstract class BaseVariable<T> : BaseGameEvent<T>, IVariable
{
    [SerializeField]
#if UNITY_EDITOR
    [Delayed]
    [OnValueChanged("OnValueChangedInInspector")]
#endif
    protected T m_value;

    [ShowInInspector]
    [ReadOnly]
    [NonSerialized]
    private T m_runtimeValue;

    public virtual T Value
    {
        get => m_runtimeValue;

        set
        {
            T oldValue = m_runtimeValue;
            m_runtimeValue = value;

            if (ValueChanged(oldValue, m_runtimeValue))
            {
                OnValueChanged(oldValue, m_runtimeValue);
            }
        }
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        Initialize();
    }
#endif

    public override void Initialize()
    {
        m_runtimeValue = m_value;
    }

#if UNITY_EDITOR
    private void OnValueChangedInInspector()
    {
        T oldValue = m_runtimeValue;
        m_runtimeValue = m_value;
        OnValueChanged(oldValue, m_runtimeValue);
    }
#endif

    protected virtual void OnValueChanged(T oldValue, T newValue)
    {
        Raise(m_runtimeValue);
    }

    public void SetValueWithoutNotify(T value)
    {
        m_runtimeValue = value;
    }

    public override string ToString()
    {
        return m_runtimeValue == null ? "null" : m_runtimeValue.ToString();
    }

    public static implicit operator T(BaseVariable<T> variable)
    {
        return variable.Value;
    }

    private bool ValueChanged(T oldValue, T newValue)
    {
        if (oldValue == null && newValue != null)
        {
            return true;
        }
        return !oldValue.Equals(newValue);
    }
}