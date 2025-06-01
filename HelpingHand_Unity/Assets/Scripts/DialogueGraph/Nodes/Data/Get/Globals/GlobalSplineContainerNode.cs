using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Get/Global Variables/Spline Container")]
public class GlobalSplineContainerNode : GlobalVariableNode<SplineContainerVariable>
{
    [Output(ShowBackingValue.Always)]
    [SerializeField]
    [InlineEditor]
    protected SplineContainerVariable m_value;

    protected override SplineContainerVariable Value => m_value;
}
