using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Variables/Globals/Float")]
public class GlobalFloatNode : GlobalVariableNode<FloatVariable>
{
    [Output(ShowBackingValue.Always)]
    [SerializeField]
    [InlineEditor]
    protected FloatVariable m_value;

    protected override FloatVariable Value => m_value;
}