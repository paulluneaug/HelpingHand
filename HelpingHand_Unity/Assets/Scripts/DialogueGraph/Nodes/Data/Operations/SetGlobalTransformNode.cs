using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Operations/Set/Globals/Transform")] 
public class SetGlobalTransformNode : SetGlobalVariableNode<Transform>
{
    [SerializeField] [InlineEditor]
    private TransformVariable m_variable;

    protected override BaseVariable<Transform> Variable => m_variable;
}