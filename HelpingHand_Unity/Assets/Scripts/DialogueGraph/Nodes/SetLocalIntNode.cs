using Sirenix.OdinInspector;

public class SetLocalIntNode : SetLocalVariableNode<int>
{
    [ShowInInspector] [ReadOnly] 
    private int Value => m_variableOut.Value;
}