using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[NodeWidth(250)] 
[NodeTint(0f, 0.4784314f, 0.6509804f)] 
[CreateNodeMenu("Data/Set/Blackboard Value")]
public class SetBlackboardValueNode : BaseNode
{
    [Input(ShowBackingValue.Never)] [SerializeField]
    private DialogueFlow m_in;

    [Input(ShowBackingValue.Unconnected)] [SerializeField]
    private object m_inValue;

    [Output] [SerializeField]
    private DialogueFlow m_out;
    
    [SerializeField] 
    private string m_key;

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (this.TryGetValueFromInputPort(nameof(m_inValue), out object outValue))
        {
            m_inValue = outValue;
        }

        GraphBlackboard.Instance.Blackboard[m_key] = m_inValue;

        await UniTask.CompletedTask;
        // Use UniTask.Yield() ?
    }
}