using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

public class RunGraphNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [SerializeField] [HideLabel]
    private SimpleGraph m_graph;

    [SerializeField] [LabelWidth(125)]
    private bool m_waitForCompletion = false;
    
    [Output]
    public DialogueFlow m_out;

    public override object GetValue(NodePort port)
    {
        return m_graph;
    }

    public override void Initialize()
    {
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        if (m_waitForCompletion)
        {
            GraphRunner runner = await GraphManager.Instance.CreateGraphRunner(m_graph);
            await runner.RunGraphAsync().AttachExternalCancellation(handler.StopToken);
        }
        else
        {
            CreateGraphRunnerAndForget().Forget();
        }
        await ContinueFlow(handler);
    }

    private async UniTaskVoid CreateGraphRunnerAndForget()
    {
        GraphRunner runner = await GraphManager.Instance.CreateGraphRunner(m_graph);
        runner.RunGraphAsync().Forget();
    }
}