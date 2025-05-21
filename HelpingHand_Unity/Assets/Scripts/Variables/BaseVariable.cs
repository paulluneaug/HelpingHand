using Events;

using Sirenix.OdinInspector;

using UnityEngine;

public interface IVariable
{
    
}

public class BaseVariable : BaseGameEvent, IVariable
{
    
}

public class BaseVariable<T> : BaseGameEvent<T>, IVariable
{
    [SerializeField]
    [Delayed]
    [OnValueChanged("OnValueChanged")]
    protected T m_value;
    
    public virtual T Value
    {
        get => m_value;
        set
        {
            T oldValue = m_value;
            m_value = value;
            if (!oldValue.Equals(m_value))
            {
                Raise(value);
            }
        }
    }

    private void OnValueChanged()
    {
        Raise(m_value);
    }

    public void SetValueWithoutNotify(T value)
    {
        m_value = value;
    }

    public override string ToString()
    {
        return m_value == null ? "null" : m_value.ToString();
    }

    public static implicit operator T(BaseVariable<T> variable)
    {
        return variable.Value;
    }
}