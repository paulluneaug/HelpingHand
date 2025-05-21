using Sirenix.OdinInspector;

public class LocalIntNode : LocalVariableNode<int>
{
    [ShowInInspector] [ReadOnly] 
    private int CurrentValue => m_variableOut.Value;
}
