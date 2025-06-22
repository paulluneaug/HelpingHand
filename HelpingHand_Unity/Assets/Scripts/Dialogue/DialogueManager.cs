using System;
using System.Diagnostics;
using System.Threading;

using Cysharp.Threading.Tasks;

using Febucci.UI;

using TMPro;

using UnityEngine;

using UnityUtility.Singletons;

using Debug = UnityEngine.Debug;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;
    public event Action OnDialogueInterrupted;

    [SerializeField] private DialoguePanelController m_panelController;

    [SerializeField]
    private TMP_Text m_uiText;

    [SerializeField]
    private TypewriterByCharacter m_typewriter;

    [SerializeField]
    private NarratorState m_narratorState;

    public NarratorState NarratorState => m_narratorState;

    private DialogueNode m_currentDialogue;
    private string m_currentDialogueTitle;
    private CancellationTokenSource m_dialogueKillCTS;
    private CancellationTokenSource m_currentCTS;

    private bool m_panelOpen = false;

    protected override void Awake()
    {
        base.Start();
        m_panelController.ClosePanel();
        m_panelOpen = false;
    }

    public void OpenDialoguePanel()
    {
        OpenPanelIfNeeded();
    }

    public async UniTask PlayDialogAsync(string dialogueTitle, string content, CancellationTokenSource stopCTS, CancellationTokenSource killCTS)
    {
        if (!string.IsNullOrEmpty(m_currentDialogueTitle) || string.IsNullOrEmpty(dialogueTitle) || string.IsNullOrEmpty(content))
        {
            return;
        }

        OpenPanelIfNeeded();

        m_currentDialogueTitle = dialogueTitle;

        m_dialogueKillCTS = killCTS;
        m_currentCTS?.Dispose();
        m_currentCTS = CancellationTokenSource.CreateLinkedTokenSource(m_dialogueKillCTS.Token, stopCTS.Token);
        
        DebugLog($"Showing content: \"{content.Truncate(30)}\"");
        
        m_typewriter.onTextShowed.AddListener(OnTextShowed);
        bool isTextShowed = false;
        m_typewriter.ShowText(content);
        
        OnDialogueStarted?.Invoke();

        if (await UniTask.WaitUntil(() => isTextShowed, PlayerLoopTiming.Update, m_currentCTS.Token).SuppressCancellationThrow())
        {
            InterruptDialogue();
            if (m_dialogueKillCTS.IsCancellationRequested)
            {
                // It is dialogue kill
                DebugLog($"Killed!");
                // m_dialogueKillCTS.Cancel(); // ?????
            }
            else
            {
                // It is pause/stop
                DebugLog($"Paused/Stopped!");
            }

            // Bubble up
            DebugLog($"Bubble up");
            throw new OperationCanceledException();
        }
        
        void OnTextShowed()
        {
            DebugLog($"On Text Showed");
            m_typewriter.onTextShowed.RemoveListener(OnTextShowed);
            isTextShowed = true;
            m_currentDialogueTitle = null;
            OnDialogueEnded?.Invoke();
        }

        void InterruptDialogue()
        {
            DebugLog($"Interrupting current dialogue ");
            m_typewriter.onTextShowed.RemoveAllListeners();
            m_typewriter.StopShowingText();
            m_currentDialogueTitle = null;
            OnDialogueInterrupted?.Invoke();
        }
    }

    public void SetDialogueKillCTS(CancellationTokenSource killCTS)
    {
        m_dialogueKillCTS = killCTS;
    }
    
    public void KillCurrentDialogue()
    {
        m_dialogueKillCTS?.Cancel();
    }

    public void ShowAllRemainingText()
    {
        m_typewriter.SkipTypewriter();
    }

    private void OpenPanelIfNeeded()
    {
        if (m_panelOpen)
        {
            return;
        }
        m_panelController.OpenPanel();
        m_panelOpen = true;
    }

    /// <summary>
    /// Debug log with header
    /// TODO: move it project-wise 
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    private void DebugLog(string log, LogType logType = LogType.Log, GameObject source = null)
    {
        string GetLogHeader()
        {
            return $"[{Time.frameCount}] <color=#ff55ff>[DialogueManager]</color> [{(!string.IsNullOrEmpty(m_currentDialogueTitle) ? m_currentDialogueTitle : "null")}]";
        }
        
        switch (logType)
        {
            case LogType.Error:
                Debug.LogError($"{GetLogHeader()} {log}", source);
                break;
            case LogType.Warning:
                Debug.LogWarning($"{GetLogHeader()} {log}", source);
                break;
            case LogType.Log:
                Debug.Log($"{GetLogHeader()} {log}", source);
                break;
        }
    }
}