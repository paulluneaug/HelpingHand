using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using UnityUtility.ObservableFields;

using XNode;

[NodeWidth(350)]
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

    [TabGroup("Content", "Neutral")]
    [Multiline(3)]
    [HideLabel]
    [SerializeField]
    private string m_content;
    
    [TabGroup("Content", "Satisfied")]
    [HideLabel, Multiline(3)]
    [SerializeField]
    private string m_contentSatisfied;
    
    [TabGroup("Content", "Annoyed")]
    [HideLabel, Multiline(3)]
    [SerializeField]
    private string m_contentAnnoyed;
    
    [TabGroup("Content", "Pissed")]
    [HideLabel, Multiline(3)]
    [SerializeField]
    private string m_contentPissed;

    [PropertySpace(SpaceBefore = 0, SpaceAfter = 8)]
    [SerializeField]
    [LabelWidth(100)]
    private bool m_canRepeat = true;

    [BoxGroup("Wait")]
    [SerializeField]
    [LabelWidth(125)]
    private float m_waitTime = 1;

    [BoxGroup("Wait")]
    [SerializeField]
    [LabelWidth(125)]
    private bool m_waitUnscaled = false;

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
        
        UniTask dialogueTask = DialogueManager.Instance.PlayDialogAsync(name, GetContent(), handler.StopToken);
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

        // Wait time
        if (Mathf.Approximately(m_waitTime, 0))
        {
            return;
        }
        
        while (true)
        {
            DebugLog($"Waiting for {m_waitTime} seconds");
            if (await UniTask.WaitForSeconds(m_waitTime, m_waitUnscaled, PlayerLoopTiming.Update, handler.StopToken).SuppressCancellationThrow())
            {
                DebugLog($"Wait interrupted");
                // The graph is being paused => We have to wait its reactivation
                await HandlePauseStop(handler);
                continue;
            }

            DebugLog($"Wait done");
            break;
        }
    }

    private string GetContent()
    {
        NarratorState narratorState = DialogueManager.Instance.NarratorState;
        if (narratorState.Satisfied.IsSet)
        {
            return string.IsNullOrEmpty(m_contentSatisfied) ? m_content : m_contentSatisfied;
        }
        
        if (narratorState.Annoyed.IsSet)
        {
            return string.IsNullOrEmpty(m_contentAnnoyed) ? m_content : m_contentAnnoyed;
        }
        
        if (narratorState.Pissed.IsSet)
        {
            return string.IsNullOrEmpty(m_contentPissed) ? m_content : m_contentPissed;
        }

        return m_content;
    }
}