using XNode;

public static partial class Extensions
{
    public static bool TryGetIntFromInputPort(this Node node, string inputPort, out int outValue)
    {
        NodePort inValuePort = node.GetInputPort(inputPort);
        if (inValuePort.ConnectionCount > 0)
        {
            if (inValuePort.TryGetInputValue(out LocalVariable<int> localInt))
            {
                outValue = localInt;
                return true;
            } 
            if (inValuePort.TryGetInputValue(out BaseVariable<int> variableInt))
            {
                outValue = variableInt;
                return true;
            }
            if (inValuePort.TryGetInputValue(out int intValue))
            {
                outValue = intValue;
                return true;
            }
        }

        outValue = 0;
        return false;
    }
}
