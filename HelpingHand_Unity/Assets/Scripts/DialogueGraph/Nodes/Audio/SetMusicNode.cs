using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Audio/Set Music")]
[NodeTint(0.6f, 0.1f, 0.1f)]
public class SetMusicNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [SerializeField]
    [HideLabel]
    private MusicState m_music;
    
    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        AudioManager.Instance.StateManager.SetMusicState(m_music); 
    }
}
