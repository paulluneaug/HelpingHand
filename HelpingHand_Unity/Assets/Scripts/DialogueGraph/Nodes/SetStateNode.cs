using Sirenix.OdinInspector;

using UnityEngine;

[NodeWidth(250)]
public class SetStateNode : SetVariableNode<bool>
{
    [SerializeField] [InlineEditor]
    private EntityState m_variable;

    protected override BaseVariable<bool> Variable => m_variable;
}