using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(250)]
public class AddToIntVariableNode : BaseNode
{
    [Input(ShowBackingValue.Never)] [SerializeField]
    private DialogueFlow m_in;

    [Input(ShowBackingValue.Always)] [SerializeField]
    private int m_increment;

    [Output] [SerializeField]
    private DialogueFlow m_out;

    [Output(ShowBackingValue.Always)] [SerializeField] [InlineEditor]
    private IntVariable m_variable;

    public override object GetValue(NodePort port)
    {
        return m_variable;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (this.TryGetValueFromInputPort(nameof(m_increment), out int outValue))
        {
            m_increment = outValue;
        }
        
        m_variable.Value += m_increment;
        
        await UniTask.CompletedTask;
    }
}