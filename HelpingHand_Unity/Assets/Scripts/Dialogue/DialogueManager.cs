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
    private CancellationTokenSource m_currentCts;

    public async UniTask PlayDialogAsync(string dialogueTitle, string content, CancellationTokenSource graphStopCTS, CancellationTokenSource killCTS)
    {
        if (!string.IsNullOrEmpty(m_currentDialogueTitle) || string.IsNullOrEmpty(dialogueTitle) || string.IsNullOrEmpty(content))
        {
            return;
        }

        m_currentDialogueTitle = dialogueTitle;

        m_dialogueKillCTS = killCTS;
        m_currentCts?.Dispose();
        m_currentCts = CancellationTokenSource.CreateLinkedTokenSource(m_dialogueKillCTS.Token, graphStopCTS.Token);
        
        DebugLog($"Play \"{content.Truncate(30)}\"");
        
        m_typewriter.onTextShowed.AddListener(OnTextShowed);
        bool isTextShowed = false;
        m_typewriter.ShowText(content);
        
        OnDialogueStarted?.Invoke();

        if (await UniTask.WaitUntil(() => isTextShowed, PlayerLoopTiming.Update, m_currentCts.Token).SuppressCancellationThrow())
        {
            InterruptDialogue();
            if (m_dialogueKillCTS.IsCancellationRequested)
            {
                // We have dialogue kill
                DebugLog($"Killed!");
                killCTS.Cancel();
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
            DebugLog($"Playing dialogue interrupted");
            m_typewriter.onTextShowed.RemoveAllListeners();
            m_typewriter.StopShowingText();
            m_currentDialogueTitle = null;
            OnDialogueInterrupted?.Invoke();
        }
    }
    
    public void KillCurrentDialogue()
    {
        m_dialogueKillCTS.Cancel();
    }

    public void ShowAllRemainingText()
    {
        m_typewriter.SkipTypewriter();
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