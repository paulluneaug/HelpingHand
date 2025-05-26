using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Graph/Run")] [NodeTint(0.6078432f, 0.2627451f, 0.6235294f)]
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

    public override void Initialize()
    {
    }
    
    public override object GetValue(NodePort port)
    {
        return m_graph;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (m_waitForCompletion)
        {
            GraphRunner runner = await GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.CreateGraphRunner(m_graph);

            await runner.RunGraphAsync().AttachExternalCancellation(handler.StopToken);
        }
        else
        {
            CreateGraphRunnerAndForget().Forget();
        }
    }

    private async UniTaskVoid CreateGraphRunnerAndForget()
    {
        GraphRunner runner = await GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.CreateGraphRunner(m_graph);
        runner.RunGraphAsync().Forget();
    }
}