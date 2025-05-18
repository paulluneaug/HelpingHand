using Sirenix.OdinInspector;

using UnityEngine;

[NodeWidth(250)]
public class SetBoolVariableNode : SetVariableNode<bool>
{
    [SerializeField] [InlineEditor]
    private BoolVariable m_variable;

    protected override BaseVariable<bool> Variable => m_variable;
}