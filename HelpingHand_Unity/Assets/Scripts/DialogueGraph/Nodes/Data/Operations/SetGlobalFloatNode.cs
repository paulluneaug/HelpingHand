using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Operations/Set/Globals/Float")]
public class SetGlobalFloatNode : SetGlobalVariableNode<float>
{
    [SerializeField]
    [InlineEditor]
    private FloatVariable m_variable;

    protected override BaseVariable<float> Variable => m_variable;
}