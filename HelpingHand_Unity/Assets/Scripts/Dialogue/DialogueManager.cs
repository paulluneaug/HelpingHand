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

    private DialogueNode m_currentDialogue;
    private string m_currentDialogueTitle;

    public async UniTask PlayDialogAsync(string dialogueTitle, string content, CancellationToken token)
    {
        if (!string.IsNullOrEmpty(m_currentDialogueTitle) || string.IsNullOrEmpty(dialogueTitle) || string.IsNullOrEmpty(content))
        {
            return;
        }

        m_currentDialogueTitle = dialogueTitle;
        
        DebugLog($"Play \"{content.Truncate(30)}\"");
        
        m_typewriter.onTextShowed.AddListener(OnTextShowed);
        bool isTextShowed = false;
        m_typewriter.ShowText(content);
        
        OnDialogueStarted?.Invoke();

        if (await UniTask.WaitUntil(() => isTextShowed, PlayerLoopTiming.Update, token).SuppressCancellationThrow())
        {
            DebugLog($"Interrupted");
            m_typewriter.onTextShowed.RemoveListener(OnTextShowed);
            m_typewriter.StopShowingText();
            m_currentDialogueTitle = null;
            OnDialogueInterrupted?.Invoke();
            // Bubble up the exception
            throw new OperationCanceledException(token);
        }
        
        void OnTextShowed()
        {
            DebugLog($"On Text Showed");
            m_typewriter.onTextShowed.RemoveListener(OnTextShowed);
            isTextShowed = true;
            m_currentDialogueTitle = null;
            OnDialogueEnded?.Invoke();
        }
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