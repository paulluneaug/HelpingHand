using System;

[Serializable]
public class GraphVariable<T>
{
    public T Value;

    public GraphVariable()
    {
        Value = default(T);
    }
    
    public GraphVariable(T value)
    {
        Value = value;
    }

    public static implicit operator T(GraphVariable<T> variable) => variable.Value;
    public static explicit operator GraphVariable<T>(T value) => new GraphVariable<T>(value);

    public override string ToString()
    {
        return Value.ToString();
    }
}
