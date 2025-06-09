using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Set/Global Variables/Int")]
public class SetGlobalIntNode : SetGlobalVariableNode<int>
{
    [SerializeField]
    [InlineEditor]
    private IntVariable m_variable;

    protected override BaseVariable<int> Variable => m_variable;
}