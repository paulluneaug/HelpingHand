using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(250)]
public class AddToLocalIntNode : BaseNode
{
    [Input(ShowBackingValue.Never)] [SerializeField]
    private DialogueFlow m_in;
    
    [Input(ShowBackingValue.Never)] [SerializeField]
    private LocalVariable<int> m_variableIn;

    [Input(ShowBackingValue.Always)] [SerializeField] 
    private int m_increment;

    [Output] [SerializeField]
    private DialogueFlow m_out;

    [Output] [SerializeField] 
    private LocalVariable<int> m_variableOut;

    [ShowInInspector] [ReadOnly] 
    private int Value => m_variableOut.Value;

    public override object GetValue(NodePort port)
    {
        return m_variableOut;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (this.TryGetIntFromInputPort(nameof(m_increment), out int outValue))
        {
            m_increment = outValue;
        }
        
        LocalVariable<int> variable = GetInputPort(nameof(m_variableIn)).GetInputValue<LocalVariable<int>>();
        variable.Value += m_increment;
        m_variableOut = variable;

        await UniTask.CompletedTask;
    }
}