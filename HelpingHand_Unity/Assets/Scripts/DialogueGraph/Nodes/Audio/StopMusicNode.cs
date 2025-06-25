using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[CreateNodeMenu("Audio/Stop Music")]
[NodeTint(0.6f, 0.1f, 0.1f)]
public class StopMusicNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    public DialogueFlow m_out;
    
    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        AudioManager.Instance.StateManager.SetMusicState(MusicState.None); 
    }
}
