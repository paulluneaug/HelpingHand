using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[CreateNodeMenu("Dialogues/Kill Dialogue")]
[NodeTint(0.0f, 0.2f, 0.0f)]
public class KillDialogueNode : BaseNode
{
    [Input(ShowBackingValue.Never)] 
    [SerializeField]
    private DialogueFlow m_in;

    [Output] 
    [SerializeField]
    private DialogueFlow m_out;
    
    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        DebugLog($"Killing dialogue!");
        DialogueManager.Instance.KillCurrentDialogue();
        // Note: audio is cancelled with the dialogue
        await UniTask.CompletedTask;
    }
}