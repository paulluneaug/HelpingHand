using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.Extensions;
using UnityUtility.ObservableFields;

using XNode;

[NodeWidth(350)]
[CreateNodeMenu("Dialogues/Dialogue")]
[NodeTint(0.2f, 0.4f, 0.2f)]
public class DialogueNode : InterruptableNode
{
    private enum DialogueNodeState
    {
        Started,
        Displayed,
        Waiting,
    }

    public bool CanRepeat => m_canRepeat;
    public ObservableField<bool> HasBeenRead => m_hasBeenRead;
    public int ReadCount => m_readCount;

    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [TabGroup("Content", "Neutral")]
    [Multiline(3)]
    [HideLabel]
    [Required]
    [SerializeField]
    private string m_contentNeutral;
    
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
    [LabelWidth(100)]
    private float m_waitTime = 1;

    [BoxGroup("Wait")]
    [SerializeField]
    [LabelWidth(100)]
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

    [BoxGroup("Debug")] 
    [ShowInInspector]
    [LabelWidth(100)]
    [ReadOnly]
    private ObservableField<bool> m_hasBeenRead = new (false);

    [BoxGroup("Debug")]
    [ShowInInspector]
    [LabelWidth(100)]
    [ReadOnly]
    private int m_readCount;

    protected override string Infos => "Display dialogue content with 4 variations. Re-execute from the beginings if interrupted.";
	// Cache
    [NonSerialized] private DialogueNodeState m_currentState;
    [NonSerialized] private CancellationTokenSource m_skipAudioCTS;
    [NonSerialized] private CancellationTokenSource m_skipWaitingCTS;

    [NonSerialized] private UniTask m_displayTask;
    [NonSerialized] private UniTask m_audioTask;

    [NonSerialized] private bool m_skipPressed;
    
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
        StartDialogueNode();

        CancellationTokenSource[] tokenSources = new CancellationTokenSource[4];
        int nextSourceIndex = 0;

        (CancellationTokenSource skipAudioCTS, CancellationTokenSource skipAudioLinkedCTS) = GetSkippableTokenSources(handler.StopToken);
        tokenSources[nextSourceIndex++] = skipAudioCTS;
        tokenSources[nextSourceIndex++] = skipAudioLinkedCTS;

        m_skipAudioCTS = skipAudioCTS;

        m_displayTask = DialogueManager.Instance.PlayDialogAsync(name, GetContent(), handler.StopToken);
        m_audioTask = m_audioEvent ?
            MakeSkippable(m_audioEvent.Play(null, skipAudioLinkedCTS.Token), skipAudioCTS, skipAudioLinkedCTS) :
            UniTask.CompletedTask;

        UniTask dialogueTask = UniTask.WhenAll(m_displayTask, m_audioTask).ContinueWith(GetFollowingTask);

        DebugLog($"Wait for dialogue end");

        if (await dialogueTask.SuppressCancellationThrow())
        {
            // Normalement le dialogue pouvait être interrompu, pas besoin de retester
            // On arrive ici si le dialogue est interrompu au milieu d'une phrase par un autre dialogue
            // ou si le graph est mis en pause 
            DebugLog($"Interrupted");
            m_hasBeenInterrupted = true;
            EndDialogueNode(tokenSources);
            return;
        }

        m_hasBeenRead.Value = true;
        m_readCount++;
        EndDialogueNode(tokenSources);

        UniTask GetFollowingTask()
        {
            m_skipPressed = false;
            m_currentState = DialogueNodeState.Waiting;

            (CancellationTokenSource skipWaitingCTS, CancellationTokenSource skipWaitingLinkedCTS) = GetSkippableTokenSources(handler.StopToken);
            tokenSources[nextSourceIndex++] = skipWaitingCTS;
            tokenSources[nextSourceIndex++] = skipWaitingLinkedCTS;

            m_skipWaitingCTS = skipWaitingCTS;

            UniTask followingTask = GameManager.Instance.GameOptionsManager.DialogueReadMode.Value switch
            {
                DialogueReadMode.Manual => UniTask.WaitUntil(() => m_skipPressed, cancellationToken: skipWaitingLinkedCTS.Token),
                DialogueReadMode.Auto => UniTask.WaitForSeconds(m_waitTime, m_waitUnscaled, PlayerLoopTiming.Update, skipWaitingLinkedCTS.Token),
                _ => throw new ArgumentOutOfRangeException(),
            };

            return MakeSkippable(followingTask, skipWaitingCTS, skipWaitingLinkedCTS);
        }
    }

    private void StartDialogueNode()
    {
        m_hasBeenInterrupted = false;
        m_currentState = DialogueNodeState.Started;
        GameManager.Instance.SkipDialogueInput.performed += OnSkipDialogue;
    }

    private void EndDialogueNode(CancellationTokenSource[] usedSources)
    {
        usedSources.ForEach(source => source?.Dispose());
        if (GameManager.ApplicationIsQuitting)
        {
            return;
        }
        GameManager.Instance.SkipDialogueInput.performed -= OnSkipDialogue;
    }

    private void OnSkipDialogue(InputAction.CallbackContext context)
    {
        m_skipPressed = true;
        switch (m_currentState)
        {
            case DialogueNodeState.Started:

                m_currentState = DialogueNodeState.Displayed;
                if (m_displayTask.Status != UniTaskStatus.Pending) // If the text is already displayed
                {
                    OnSkipDialogue(context);
                    return;
                }

                DialogueManager.Instance.ShowAllRemainingText();
                break;

            case DialogueNodeState.Displayed:
                m_currentState = DialogueNodeState.Waiting;
                if (m_audioTask.Status != UniTaskStatus.Pending) // If the audio is already finished
                {
                    OnSkipDialogue(context);
                    return;
                }
                m_skipAudioCTS.Cancel();
                break;

            case DialogueNodeState.Waiting:
                m_skipWaitingCTS.Cancel();
                break;

            default:
                break;
        }
    }

    private (CancellationTokenSource skipCTS, CancellationTokenSource linkedCTS) GetSkippableTokenSources(CancellationToken token)
    {
        CancellationTokenSource skipSource = new CancellationTokenSource();
        return (skipSource, CancellationTokenSource.CreateLinkedTokenSource(token, skipSource.Token));
    }

    private async UniTask MakeSkippable(UniTask task, CancellationTokenSource skipCTS, CancellationTokenSource linkedCTS)
    {
        UniTask skipTask = UniTask.WaitUntilCanceled(skipCTS.Token);

        (bool isCancelled, int _) = await UniTask.WhenAny(task, skipTask).SuppressCancellationThrow();

        if (!isCancelled) // The main task succeeded
        {
            return;
        }

        if (skipCTS.IsCancellationRequested) // Only the skip token was cancelled
        {
            return;
        }

        // The handler's StopToken was cancelled
        // so we bubble up the cancellation exception
        throw new OperationCanceledException();
    }
    
    private string GetContent()
    {
        NarratorState narratorState = DialogueManager.Instance.NarratorState;
        if (narratorState.Satisfied.IsSet)
        {
            return string.IsNullOrEmpty(m_contentSatisfied) ? m_contentNeutral : m_contentSatisfied;
        }
        
        if (narratorState.Annoyed.IsSet)
        {
            return string.IsNullOrEmpty(m_contentAnnoyed) ? m_contentNeutral : m_contentAnnoyed;
        }
        
        if (narratorState.Pissed.IsSet)
        {
            return string.IsNullOrEmpty(m_contentPissed) ? m_contentNeutral : m_contentPissed;
        }

        return m_contentNeutral;
    }
}