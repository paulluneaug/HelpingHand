using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[CreateNodeMenu("Flow/Loop")]
[NodeTint(0.4f, 0.2f, 0f)] 
public class LoopNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Input] 
    [SerializeField]
    private DialogueFlow m_breakLoop;
    
    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    private bool m_isLooping;

    public override void Initialize()
    {
        base.Initialize();
        m_isLooping = true;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (inPort.fieldName.Equals(nameof(m_breakLoop)))
        {
            DebugLog($"Loop broken");
            m_isLooping = false;
        }
    }
    
    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        while (m_isLooping)
        {
            DebugLog("Looping");
            await base.ContinueFlow(handler, inPort);
        }
    }
}