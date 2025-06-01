using Sirenix.OdinInspector;

[CreateNodeMenu("Data/Set/Graph Variables/Bool")]
public class SetGraphIntNode : SetGraphVariableNode<int>
{
    [ShowInInspector]
    [ReadOnly]
    private int Value => m_variableOut.Value;
}