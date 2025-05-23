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
    [OnValueChanged("OnValueChanged")]
#endif
    protected T m_value;

#if UNITY_EDITOR
    [ShowInInspector] [ReadOnly] [NonSerialized]
    private T m_runtimeValue;
#endif

    public virtual T Value
    {
        get
        {
#if UNITY_EDITOR
            return m_runtimeValue;
#else
            return m_value;
#endif
            
        } 
        set
        {
#if UNITY_EDITOR
            T oldValue = m_runtimeValue;
            m_runtimeValue = value;
            if (!oldValue.Equals(m_runtimeValue))
            {
                Raise(m_runtimeValue);
            }
#else
            T oldValue = m_value;
            m_value = value;
            if (!oldValue.Equals(m_value))
            {
                Raise(m_value);
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
    private void OnValueChanged()
    {
        m_runtimeValue = m_value;
        Raise(m_runtimeValue);
    }
#endif

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
}