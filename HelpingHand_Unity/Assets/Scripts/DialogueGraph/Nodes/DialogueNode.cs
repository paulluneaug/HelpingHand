using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using UnityUtility.ObservableFields;

using XNode;

[NodeWidth(300)]
public class DialogueNode : InterruptableNode
{
    [Input] [SerializeField]
    private DialogueFlow m_in;

    [Output] [SerializeField]
    private DialogueFlow m_out;

    [BoxGroup("Content")] 
    [HideLabel, Multiline(3)] [SerializeField]
    private string m_content;

    [BoxGroup("Content")] 
    [SerializeField] [LabelWidth(100)]
    private bool m_multipleReads;

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
    [BoxGroup("Audio")] [HideLabel] [InlineEditor] [SerializeField]
    private AudioEvent m_audioEvent;

    [FoldoutGroup("Debug")] [ShowInInspector, LabelWidth(125), ReadOnly]
    private ObservableField<bool> m_hasBeenRead;

    [FoldoutGroup("Debug")] [ShowInInspector, LabelWidth(125), ReadOnly]
    private int m_readCount;

    public string Content => m_content;
    public bool MultipleReads => m_multipleReads;
    public ObservableField<bool> HasBeenRead => m_hasBeenRead;
    public int ReadCount => m_readCount;
    
    private bool m_isReadingText;

    protected override void Init()
    {
        base.Init();
        m_description = "Display the content. Loops back if interrupted";
    }

    public override void Initialize()
    {
        m_hasBeenRead.Value = false;
        m_isReadingText = false;
        m_readCount = 0;
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
            DebugLog($"End");
            await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_out)));
        }
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        DebugLog($"Play");
        m_hasBeenInterrupted = false;
        m_isReadingText = true;
        
        // Todo rendre awaitable
        DialogueManager.Instance.PlayDialog(this);
        DialogueManager.Instance.OnDialogueEnded += OnDialogueEnded;
        DebugLog($"Wait for end");
        
        bool isCancelled = await UniTask.WhenAll(
            m_audioEvent? m_audioEvent.Play(null, handler.StopToken) : UniTask.CompletedTask,
            WaitForDialogueEnd(handler)
            ).SuppressCancellationThrow();
        
        if (isCancelled)
        {
            // Normalement le dialogue pouvait être interrompu, pas besoin de retester
            // On arrive ici si le dialogue est interrompu au milieu d'une phrase par un autre dialogue
            // ou si le graph est mis en pause 
            DebugLog($"Interrupted");
            m_hasBeenInterrupted = true;
            DialogueManager.Instance.InterruptDialogue();
        }
    }

    private async UniTask WaitForDialogueEnd(GraphRunnerHandler handler)
    {
        await UniTask.WaitUntil(() => !m_isReadingText || m_hasBeenInterrupted, PlayerLoopTiming.Update, handler.StopToken);
    }

    private void OnDialogueEnded()
    {
        DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnded;
        m_hasBeenRead.Value = true;
        m_isReadingText = false;
        m_readCount++;
    }

    public void ResetReadCount()
    {
        m_readCount = 0;
    }
}