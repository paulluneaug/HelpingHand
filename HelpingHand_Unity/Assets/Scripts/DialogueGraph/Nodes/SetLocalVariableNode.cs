using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

public abstract class SetLocalVariableNode<T> : BaseNode
{
    [Input(ShowBackingValue.Never)] [SerializeField]
    protected DialogueFlow m_in;

    [Input(ShowBackingValue.Never)] [SerializeField]
    protected LocalVariable<T> m_inVariable;

    [Input(ShowBackingValue.Always)] [SerializeField] 
    protected T m_value;

    [Output] [SerializeField]
    protected DialogueFlow m_out;

    [Output(ShowBackingValue.Never)] [SerializeField] 
    protected LocalVariable<T> m_variableOut;
    

    public override object GetValue(NodePort port)
    {
        return m_variableOut;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (this.TryGetValueFromInputPort(nameof(m_value), out T outValue))
        {
            m_value = outValue;
        }

        m_inVariable = GetInputPort(nameof(m_inVariable)).GetInputValue<LocalVariable<T>>();

        m_inVariable.Value = m_value;

        m_variableOut = m_inVariable;

        await UniTask.CompletedTask;
    }
}