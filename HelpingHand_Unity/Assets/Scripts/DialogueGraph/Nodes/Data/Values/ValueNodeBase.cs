using XNode;

[NodeTint(0f, 0.4784314f, 0.6509804f)]
public abstract class ValueNodeBase<T> : BaseNode
{
    protected abstract T Value { get; }

    public override object GetValue(NodePort port)
    {
        return Value;
    }
}