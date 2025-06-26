using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[CreateNodeMenu("Graph/Stop All")]
[NodeTint(0.6078432f, 0.2627451f, 0.6235294f)]
[NodeWidth(250)]
public class StopAllGraphsNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.StopSequence(handler);
    }
}