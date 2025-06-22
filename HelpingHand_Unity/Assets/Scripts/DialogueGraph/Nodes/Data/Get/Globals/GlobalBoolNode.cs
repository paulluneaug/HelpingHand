using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Get/Global Variables/Bool")]
public class GlobalBoolNode : GlobalVariableNode<BoolVariable>
{
    [Output(ShowBackingValue.Always)]
    [SerializeField]
    [InlineEditor]
    protected BoolVariable m_value;

    protected override BoolVariable Value => m_value;
}