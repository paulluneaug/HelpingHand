using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Get/Global Variables/Float")]
public class GlobalFloatNode : GlobalVariableNode<FloatVariable>
{
    [Output(ShowBackingValue.Always)]
    [SerializeField]
    [InlineEditor]
    protected FloatVariable m_value;

    protected override FloatVariable Value => m_value;
}