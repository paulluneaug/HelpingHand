using Sirenix.OdinInspector;

[CreateNodeMenu("Data/Operations/Set/Graphs/Float")] 
public class SetGraphFloatNode : SetGraphVariableNode<float>
{
    [ShowInInspector] [ReadOnly] 
    private float Value => m_variableOut.Value;
}