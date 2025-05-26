using System;

[Serializable]
public class GraphVariable<T>
{
    public T Value;

    public GraphVariable()
    {
        Value = default;
    }

    public GraphVariable(T value)
    {
        Value = value;
    }

    public static implicit operator T(GraphVariable<T> variable)
    {
        return variable.Value;
    }

    public static explicit operator GraphVariable<T>(T value)
    {
        return new GraphVariable<T>(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
