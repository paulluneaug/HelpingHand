using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[NodeWidth(250)]
public abstract class SetVariableNode<T> : BaseNode
{
    [Input(ShowBackingValue.Never)] 
    public DialogueFlow m_in;

    [Input]
    public T m_inValue;

    [Output] public DialogueFlow m_out;
    
    protected abstract BaseVariable<T> Variable { get; }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        T inValue = m_inValue;
        NodePort inValuePort = GetInputPort(nameof(m_inValue));
        if (inValuePort.ConnectionCount > 0)
        {
            inValue = inValuePort.GetInputValue<T>();
            Debug.Log($"Reading value {inValue} from input port");
        }
        else
        {
            Debug.Log($"Reading value {inValue} from node");
        }

        Variable.Value = inValue;
    }
}