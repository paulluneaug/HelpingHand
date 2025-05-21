using XNode;

public static partial class Extensions
{
    public static bool TryGetValueFromInputPort<T>(this Node node, string inputPort, out T outValue)
    {
        NodePort inValuePort = node.GetInputPort(inputPort);
        if (inValuePort.ConnectionCount > 0)
        {
            if (inValuePort.TryGetInputValue(out GraphVariable<T> local))
            {
                outValue = local;
                return true;
            } 
            if (inValuePort.TryGetInputValue(out BaseVariable<T> variable))
            {
                outValue = variable;
                return true;
            }
            if (inValuePort.TryGetInputValue(out T value))
            {
                outValue = value;
                return true;
            }
        }

        outValue = default(T);
        return false;
    }
}
