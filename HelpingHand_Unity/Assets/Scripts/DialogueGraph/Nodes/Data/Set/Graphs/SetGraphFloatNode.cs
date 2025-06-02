using Sirenix.OdinInspector;

[CreateNodeMenu("Data/Set/Graph Variables/Float")]
public class SetGraphFloatNode : SetGraphVariableNode<float>
{
    [ShowInInspector]
    [ReadOnly]
    private float Value => m_variableOut.Value;
}