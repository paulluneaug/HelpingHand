using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[CreateNodeMenu("Go to credits")]
[NodeTint(.7f, .2f, .2f)]
public class GoToCreditsNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        // Play credits
        GameManager.Instance.ReturnToMainMenu();
        GameManager.Instance.CanvasManager.OpenCredits();
    }
}