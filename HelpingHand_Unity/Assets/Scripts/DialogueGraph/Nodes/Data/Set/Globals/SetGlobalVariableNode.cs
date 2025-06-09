using Cysharp.Threading.Tasks;

using XNode;

[NodeWidth(250)]
[NodeTint(0f, 0.4784314f, 0.6509804f)]
public abstract class SetGlobalVariableNode<T> : BaseNode
{
    [Input(ShowBackingValue.Never)]
    public DialogueFlow m_in;

    [Input]
    public T m_inValue;

    [Output] public DialogueFlow m_out;

    protected abstract BaseVariable<T> Variable { get; }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (this.TryGetValueFromInputPort(nameof(m_inValue), out T outValue))
        {
            m_inValue = outValue;
        }

        DebugLog($"Setting global variable [{Variable.name}] to value [{m_inValue}]");
        Variable.Value = m_inValue;

        await UniTask.CompletedTask;
    }
}