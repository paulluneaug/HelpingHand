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

#if UNITY_EDITOR
    [ShowInInspector]
    [ReadOnly]
    [NonSerialized]
    private T m_runtimeValue;
#endif

    public virtual T Value
    {
        get =>
#if UNITY_EDITOR
            m_runtimeValue;
#else
            m_value;
#endif

        set
        {
#if UNITY_EDITOR
            T oldValue = m_runtimeValue;
            m_runtimeValue = value;

            if (ValueChanged(oldValue, m_runtimeValue))
            {
                OnValueChanged(oldValue, m_runtimeValue);
            }
#else
            T oldValue = m_value;
            m_value = value;

            if (ValueChanged(oldValue, m_value))
            {
                OnValueChanged(oldValue, m_value);
            }
#endif
        }
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        Initialize();
    }
#endif

#if UNITY_EDITOR
    protected virtual void Initialize()
    {
        m_runtimeValue = m_value;
    }
#endif

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
#if UNITY_EDITOR
        m_runtimeValue = value;
#else
        m_value = value;
#endif
    }

    public override string ToString()
    {
#if UNITY_EDITOR
        return m_runtimeValue == null ? "null" : m_runtimeValue.ToString();
#else
        return m_value == null ? "null" : m_value.ToString();
#endif
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