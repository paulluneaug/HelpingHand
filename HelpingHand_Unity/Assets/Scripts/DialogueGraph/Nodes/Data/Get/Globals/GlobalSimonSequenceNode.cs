using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Get/Global Variables/Simon Sequence")] 
public class GlobalSimonSequenceNode : GlobalVariableNode<SimonSequenceVariable>
{
    [Output(ShowBackingValue.Always)] [SerializeField] [InlineEditor]
    protected SimonSequenceVariable m_value;

    protected override SimonSequenceVariable Value => m_value;
}