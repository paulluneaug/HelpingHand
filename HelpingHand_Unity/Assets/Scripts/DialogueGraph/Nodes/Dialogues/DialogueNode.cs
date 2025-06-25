using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.InputSystem;

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

    [Space]
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
    [NonSerialized] private CancellationTokenSource m_audioSkipCTS;
    [NonSerialized] private CancellationTokenSource m_audioLinkedCTS;
    [NonSerialized] private CancellationTokenSource m_waitingSkipCTS;
    [NonSerialized] private CancellationTokenSource m_waitingLinkedCTS;

    [NonSerialized] private UniTask m_displayTask;
    [NonSerialized] private UniTask m_audioTask;

    [NonSerialized] private bool m_skipPressed;

    public override void Initialize()
    {
        base.Initialize();
        m_hasBeenRead.Value = false;
        m_readCount = 0; // Quid si on relance plusieurs fois le même graph, le compteur est reset
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        // TODO gérer m_hasBeenKilled
        if (m_hasBeenKilled)
        {
            DebugLog($"Has been killed");
            return;
        }

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
        await base.ExecuteNode(handler, inPort);
        if (m_hasBeenKilled)
        {
            DebugLog($"Dialogue has already been killed");
            return;
        }

        DebugLog($"Play");
        StartDialogueNode();

        m_audioSkipCTS?.Dispose();
        m_audioSkipCTS = new CancellationTokenSource();
        m_audioLinkedCTS?.Dispose();
        m_audioLinkedCTS = m_audioSkipCTS.Token.LinkWith(m_killCTS.Token, handler.StopToken);
        m_waitingSkipCTS?.Dispose();
        m_waitingSkipCTS = new CancellationTokenSource();
        m_waitingLinkedCTS?.Dispose();
        m_waitingLinkedCTS = m_waitingSkipCTS.Token.LinkWith(m_killCTS.Token, handler.StopToken);

        m_displayTask = DialogueManager.Instance.PlayDialogAsync(name, GetContent(), handler.StopSource, m_killCTS);
        m_audioTask = m_audioEvent ?
            MakeSkippable(m_audioEvent.Play(null, m_audioLinkedCTS.Token), m_audioSkipCTS, m_audioLinkedCTS, m_killStopCTS) :
            UniTask.CompletedTask;

        UniTask task = UniTask.WhenAll(m_displayTask, m_audioTask).ContinueWith(GetWaitingTask);

        DebugLog($"Wait for dialogue to end");

        if (await task.SuppressCancellationThrow())
        {
            // Normalement le dialogue pouvait être interrompu, pas besoin de retester
            // On arrive ici si le dialogue est interrompu au milieu d'une phrase par un autre dialogue
            // ou si le graph est mis en pause 
            if (handler.StopSource.IsCancellationRequested)
            {
                DebugLog($"Interrupted by stop/pause");
                m_hasBeenInterrupted = true;
                EndDialogueNode();
            }
            else if (m_waitingSkipCTS.IsCancellationRequested || m_audioSkipCTS.IsCancellationRequested)
            {
                DebugLog("Skipped");
                EndDialogueNode();
            }
            else
            {
                Debug.Assert(m_killCTS.IsCancellationRequested);
                DebugLog($"Killed");
                EndDialogueNode();
                // Kill the branch (don't continue the flow)
                m_hasBeenKilled = true;
            }

            return;
        }


        // We signal the skip token to stop skip tasks from running
        m_waitingSkipCTS.Cancel();
        m_audioSkipCTS.Cancel();

        m_hasBeenRead.Value = true;
        m_readCount++;
        EndDialogueNode();

        UniTask GetWaitingTask()
        {
            DebugLog($"Waiting {(GameManager.Instance.GameOptionsManager.DialogueReadMode.Value == DialogueReadMode.Auto ? m_waitTime + "s" : "for next button")} to continue...");
            m_skipPressed = false;
            m_currentState = DialogueNodeState.Waiting;

            UniTask followingTask = GameManager.Instance.GameOptionsManager.DialogueReadMode.Value switch
            {
                DialogueReadMode.Manual => UniTask.WaitUntil(() => m_skipPressed, cancellationToken: m_killStopCTS.Token),
                DialogueReadMode.Auto => UniTask.WaitForSeconds(m_waitTime, m_waitUnscaled, cancellationToken: m_killStopCTS.Token),
                _ => throw new ArgumentOutOfRangeException(),
            };

            return MakeSkippable(followingTask, m_waitingSkipCTS, m_waitingLinkedCTS, m_killStopCTS);
        }
    }

    private void StartDialogueNode()
    {
        m_hasBeenInterrupted = false;
        m_currentState = DialogueNodeState.Started;
        GameManager.Instance.SkipDialogueInput.AddDownListener(OnSkipDialogue);
    }

    private void EndDialogueNode()
    {
        DialogueManager.Instance.SetDialogueKillCTS(null);

        if (GameManager.ApplicationIsQuitting)
        {
            return;
        }
        GameManager.Instance.SkipDialogueInput.RemoveDownListener(OnSkipDialogue);
    }

    private void OnSkipDialogue()
    {
        m_skipPressed = true;
        switch (m_currentState)
        {
            case DialogueNodeState.Started:

                m_currentState = DialogueNodeState.Displayed;
                if (m_displayTask.Status != UniTaskStatus.Pending) // If the text is already displayed
                {
                    OnSkipDialogue();
                    return;
                }

                DialogueManager.Instance.ShowAllRemainingText();
                break;

            case DialogueNodeState.Displayed:
                m_currentState = DialogueNodeState.Waiting;
                if (m_audioTask.Status != UniTaskStatus.Pending) // If the audio is already finished
                {
                    OnSkipDialogue();
                    return;
                }
                m_audioSkipCTS.Cancel();
                break;

            case DialogueNodeState.Waiting:
                m_waitingSkipCTS.Cancel();
                break;

            default:
                break;
        }
    }

    private async UniTask MakeSkippable(UniTask task, CancellationTokenSource skipCTS, CancellationTokenSource skipStopCTS, CancellationTokenSource killStopCTS)
    {
        // UniTask skipTask = UniTask.WaitUntilCanceled(linkedSkipCTS.Token);
        UniTask skipTask = UniTask.WaitUntilCanceled(skipStopCTS.Token);
        UniTask killStopTask = UniTask.WaitUntilCanceled(killStopCTS.Token);

        (bool isCancelled, int _) = await UniTask.WhenAny(task, skipTask, killStopTask).SuppressCancellationThrow();

        if (killStopCTS.IsCancellationRequested)
        {
            DebugLog($"MakeSkippable: Killed or stopped");
            throw new OperationCanceledException();
        }

        if (skipCTS.IsCancellationRequested) // The skip token was cancelled
        {
            DebugLog($"Main task skipped");
            throw new OperationCanceledException();
        }

        if (!isCancelled) // The main task succeeded
        {
            DebugLog($"Main task succeeded");
            return;
        }

        // A token in the task was cancelled (eg. the kill or stop tokens)
        // so we bubble up the cancellation exception
        DebugLog($"MakeSkippable: Killed or stopped");
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

    public override void Dispose()
    {
        base.Dispose();

        m_audioSkipCTS?.Dispose();
        m_audioLinkedCTS?.Dispose();
        m_waitingSkipCTS?.Dispose();
        m_waitingLinkedCTS?.Dispose();
    }
}