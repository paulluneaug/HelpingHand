using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public class StartNode : BaseNode
{
    [Output] [SerializeField] 
    private DialogueFlow m_out;
    
    [Output] [SerializeField] 
    private GraphRunnerHandler m_handler;

    [SerializeField] 
    private int m_priority;

    protected override void Init()
    {
        base.Init();
        m_description = "Start the graph here";
    }

    public override object GetValue(NodePort port)
    {
        return m_handler;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        handler.Priority = m_priority;
        m_handler = handler;
    }
}