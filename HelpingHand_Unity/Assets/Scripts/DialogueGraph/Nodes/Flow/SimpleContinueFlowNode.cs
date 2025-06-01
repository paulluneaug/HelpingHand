using UnityEngine;

[NodeTint(0.4f, 0.2f, 0f)] [CreateNodeMenu("Flow/Continue")] [NodeWidth(150)]
public class SimpleContinueFlowNode : BaseNode
{
    [Input(ShowBackingValue.Never)] [SerializeField]
    private DialogueFlow m_in;

    [Output(ShowBackingValue.Never)] [SerializeField]
    private DialogueFlow m_out;

}