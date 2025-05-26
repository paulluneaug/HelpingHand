using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Operations/Set/Globals/State")]
public class SetStateNode : SetGlobalVariableNode<bool>
{
    [SerializeField]
    [InlineEditor]
    private EntityState m_variable;

    protected override BaseVariable<bool> Variable => m_variable;
}