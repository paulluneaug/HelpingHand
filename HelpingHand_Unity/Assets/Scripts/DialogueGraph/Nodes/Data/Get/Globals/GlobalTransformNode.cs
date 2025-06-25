using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Get/Global Variables/Transform")]
public class GlobalTransformNode : GlobalVariableNode<TransformVariable>
{
    [Output(ShowBackingValue.Always)]
    [SerializeField]
    [InlineEditor]
    protected TransformVariable m_value;

    protected override TransformVariable Value => m_value;
}