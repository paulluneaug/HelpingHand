using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Set/Global Variables/Bool")]
public class SetGlobalBoolNode : SetGlobalVariableNode<bool>
{
    [SerializeField]
    [InlineEditor]
    [HideLabel]
    private BoolVariable m_variable;

    protected override BaseVariable<bool> Variable => m_variable;
}