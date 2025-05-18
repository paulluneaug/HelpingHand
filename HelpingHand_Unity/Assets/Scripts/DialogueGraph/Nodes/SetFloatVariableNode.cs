using Sirenix.OdinInspector;

using UnityEngine;

[NodeWidth(250)]
public class SetFloatVariableNode : SetVariableNode<float>
{
    [SerializeField] [InlineEditor]
    private FloatVariable m_variable;

    protected override BaseVariable<float> Variable => m_variable;
}