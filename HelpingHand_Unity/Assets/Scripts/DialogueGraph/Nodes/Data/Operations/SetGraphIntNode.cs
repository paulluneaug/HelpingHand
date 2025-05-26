using Sirenix.OdinInspector;

[CreateNodeMenu("Data/Operations/Set/Graphs/Bool")]
public class SetGraphIntNode : SetGraphVariableNode<int>
{
    [ShowInInspector]
    [ReadOnly]
    private int Value => m_variableOut.Value;
}