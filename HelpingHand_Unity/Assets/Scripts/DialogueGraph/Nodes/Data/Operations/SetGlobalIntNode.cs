using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Operations/Set/Globals/Int")]
public class SetGlobalIntNode : SetGlobalVariableNode<int>
{
    [SerializeField]
    [InlineEditor]
    private IntVariable m_variable;

    protected override BaseVariable<int> Variable => m_variable;
}