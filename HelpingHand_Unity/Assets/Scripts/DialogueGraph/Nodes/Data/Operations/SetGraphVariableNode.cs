using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[NodeWidth(250)]
[NodeTint(0f, 0.4784314f, 0.6509804f)]
public abstract class SetGraphVariableNode<T> : BaseNode
{
    [Input(ShowBackingValue.Never)]
    [SerializeField]
    protected DialogueFlow m_in;

    [Input(ShowBackingValue.Never)]
    [SerializeField]
    protected GraphVariable<T> m_inVariable;

    [Input(ShowBackingValue.Unconnected)]
    [SerializeField]
    protected T m_value;

    [Output]
    [SerializeField]
    protected DialogueFlow m_out;

    [Output(ShowBackingValue.Never)]
    [SerializeField]
    protected GraphVariable<T> m_variableOut;

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

        m_inVariable = GetInputPort(nameof(m_inVariable)).GetInputValue<GraphVariable<T>>();

        m_inVariable.Value = m_value;

        m_variableOut = m_inVariable;

        await UniTask.CompletedTask;
    }
}