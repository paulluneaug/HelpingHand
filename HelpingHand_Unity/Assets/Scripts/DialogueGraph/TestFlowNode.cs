using Cysharp.Threading.Tasks;

using XNode;

public class TestFlowNode : BaseNode
{
    [Input] public DialogueFlow m_in;
    [Input] public DialogueFlow m_resetIn;
    [Output] public DialogueFlow m_out;
    
    public override void Initialize()
    {
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort port)
    {
        DebugLog($"Execute port={port.fieldName}");
        if (port == GetInputPort("m_in"))
        {
            DebugLog($"Coming from in input");
        }
        else if (port == GetInputPort("m_resetIn"))
        {
            DebugLog($"Coming from resetIn");
        }
        
        await ContinueFlow(handler);
    }
}
