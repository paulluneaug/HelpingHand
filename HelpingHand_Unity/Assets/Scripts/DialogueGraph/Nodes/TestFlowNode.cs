using Cysharp.Threading.Tasks;

using XNode;

public class TestFlowNode : BaseNode
{
    [Input] public DialogueFlow m_in;
    [Input] public DialogueFlow m_resetIn;
    [Output] public DialogueFlow m_out;

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        DebugLog($"Execute port={inPort.fieldName}");
        if (inPort == GetInputPort("m_in"))
        {
            DebugLog($"Coming from in input");
        }
        else if (inPort == GetInputPort("m_resetIn"))
        {
            DebugLog($"Coming from resetIn");
        }
    }
}
