using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Splines;

[CreateNodeMenu("Data/Operations/Set/Globals/Spline Container")] 
public class SetGlobalSplineContainerNode : SetGlobalVariableNode<SplineContainer>
{
    [SerializeField] [InlineEditor]
    private SplineContainerVariable m_variable;

    protected override BaseVariable<SplineContainer> Variable => m_variable;
}