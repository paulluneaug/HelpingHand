using System;

[Serializable]
public class LocalVariable<T>
{
    public T Value;

    public LocalVariable()
    {
        Value = default(T);
    }
    
    public LocalVariable(T value)
    {
        Value = value;
    }

    public static implicit operator T(LocalVariable<T> variable) => variable.Value;
    public static explicit operator LocalVariable<T>(T value) => new LocalVariable<T>(value);

    public override string ToString()
    {
        return Value.ToString();
    }
}
