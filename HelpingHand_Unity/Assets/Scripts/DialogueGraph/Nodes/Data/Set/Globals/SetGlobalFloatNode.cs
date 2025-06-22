using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Set/Global Variables/Float")]
public class SetGlobalFloatNode : SetGlobalVariableNode<float>
{
    [SerializeField]
    [InlineEditor]
    private FloatVariable m_variable;

    protected override BaseVariable<float> Variable => m_variable;
}