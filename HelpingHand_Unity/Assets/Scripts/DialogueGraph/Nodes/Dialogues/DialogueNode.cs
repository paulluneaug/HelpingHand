using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Serialization;

using UnityUtility.ObservableFields;

using XNode;

[NodeWidth(300)]
[CreateNodeMenu("Dialogues/Dialogue")]
[NodeTint(0.2f, 0.4f, 0.2f)]
public class DialogueNode : InterruptableNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [BoxGroup("Content")]
    [HideLabel, Multiline(3)]
    [SerializeField]
    private string m_content;

    [FormerlySerializedAs("m_multipleReads")]
    [BoxGroup("Content")]
    [SerializeField]
    [LabelWidth(100)]
    private bool m_canRepeat = true;

    [ShowIf("@m_audioEvent == null")]
    [BoxGroup("Audio")]
    [Button("Add Audio Event")]
    private void AddAudioEvent()
    {
        m_audioEvent = ScriptableObject.CreateInstance<AudioEvent>();
        m_audioEvent.name = $"Audio_{name}";
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(m_audioEvent, $"Assets/Resources/AudioEvents/{m_audioEvent.name}.asset");
#endif
    }

    [HideIf("@m_audioEvent == null")]
    [BoxGroup("Audio")]
    [HideLabel]
    [InlineEditor]
    [SerializeField]
    private AudioEvent m_audioEvent;

    [FoldoutGroup("Debug")] [ShowInInspector, LabelWidth(125), ReadOnly]
    private ObservableField<bool> m_hasBeenRead = new (false);

    [FoldoutGroup("Debug")]
    [ShowInInspector, LabelWidth(125), ReadOnly]
    private int m_readCount;

    public string Content => m_content;
    public bool CanRepeat => m_canRepeat;
    public ObservableField<bool> HasBeenRead => m_hasBeenRead;
    public int ReadCount => m_readCount;
    
    protected override void Init()
    {
        base.Init();
        m_description = "Display the content. Loops back if interrupted";
    }

    public override void Initialize()
    {
        m_hasBeenRead.Value = false;
        m_readCount = 0; // Quid si on relance plusieurs fois le même graph, le compteur est reset
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        if (m_hasBeenInterrupted)
        {
            DebugLog($"Has been interrupted");
            m_hasBeenInterrupted = false;

            // re-execute the node
            await Execute(handler);
        }
        else
        {
            DebugLog($"Dialogue has ended normally");
            await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_out)));
        }
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        DebugLog($"Play");
        m_hasBeenInterrupted = false;
        
        UniTask dialogueTask = DialogueManager.Instance.PlayDialogAsync(name, m_content, handler.StopToken);
        UniTask audioTask = m_audioEvent ? m_audioEvent.Play(null, handler.StopToken) : UniTask.CompletedTask;
        
        DebugLog($"Wait for dialogue end");
        
        if (await UniTask.WhenAll(dialogueTask, audioTask).SuppressCancellationThrow())
        {
            // Normalement le dialogue pouvait être interrompu, pas besoin de retester
            // On arrive ici si le dialogue est interrompu au milieu d'une phrase par un autre dialogue
            // ou si le graph est mis en pause 
            DebugLog($"Interrupted");
            m_hasBeenInterrupted = true;
            return;
        }
        
        m_hasBeenRead.Value = true;
        m_readCount++;
    }
}