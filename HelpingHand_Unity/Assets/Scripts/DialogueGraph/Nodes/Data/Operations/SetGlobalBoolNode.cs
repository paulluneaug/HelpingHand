using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Operations/Set/Globals/Bool")] 
public class SetGlobalBoolNode : SetGlobalVariableNode<bool>
{
    [SerializeField] [InlineEditor]
    private BoolVariable m_variable;

    protected override BaseVariable<bool> Variable => m_variable;
}