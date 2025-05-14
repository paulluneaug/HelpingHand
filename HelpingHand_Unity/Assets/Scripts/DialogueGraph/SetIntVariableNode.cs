using Sirenix.OdinInspector;

using UnityEngine;

[NodeWidth(250)]
public class SetIntVariableNode : SetVariableNode<int>
{
    [SerializeField] [InlineEditor]
    private IntVariable m_variable;

    protected override BaseVariable<int> Variable => m_variable;
}