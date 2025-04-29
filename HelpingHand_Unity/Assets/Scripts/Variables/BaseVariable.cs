using Events;

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

    protected virtual T ClampValue(T value)
    {
        return value;
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