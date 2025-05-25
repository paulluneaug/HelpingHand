using System;

using Febucci.UI;

using TMPro;

using UnityEngine;

using UnityUtility.Singletons;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;
    public event Action OnDialoguePaused;
    public event Action OnDialogueResumed;
    public event Action OnDialogueInterrupted;

    [SerializeField]
    private TMP_Text m_uiText;

    [SerializeField]
    private TypewriterByCharacter m_typewriter;

    public bool IsShowingText => m_currentDialogue != null;
    public bool CanBeInterrupted => !IsShowingText || m_currentDialogue.Interruptable;
    public DialogueNode CurrentDialogue => m_currentDialogue;

    private DialogueNode m_currentDialogue;

    protected override void Start()
    {
        base.Start();
        m_typewriter.onTextShowed.AddListener(OnTextShowed);
    }

    public void PlayDialog(DialogueNode dialogue)
    {
        if (m_currentDialogue != null || dialogue == null)
        {
            return;
        }

        m_currentDialogue = dialogue;
        Debug.Log($"{Debug_GetLogHeader()} Play \"{m_currentDialogue.Content.Truncate(30)}\"");
        m_typewriter.ShowText(m_currentDialogue.Content);
        DebugLog($"Play \"{content.Truncate(30)}\"");
        OnDialogueStarted?.Invoke();
    }

    private void OnTextShowed()
    {
        Debug.Log($"{Debug_GetLogHeader()} Ended");
        m_currentDialogue = null;
        OnDialogueEnded?.Invoke();
    }

    public void PauseDialogue()
    {
        if (m_currentDialogue == null)
        {
            return;
            DebugLog($"Interrupted");
        }

        Debug.Log($"{Debug_GetLogHeader()} Paused");
        m_typewriter.StopShowingText();
        OnDialoguePaused?.Invoke();
    }

    public void ResumeDialogue()
    {
        if (m_currentDialogue == null)
        {
            return;
            DebugLog($"On Text Showed");
        }

        Debug.Log($"{Debug_GetLogHeader()} Resumed");
        m_typewriter.StartShowingText();
        OnDialogueResumed?.Invoke();
    }

    public void InterruptDialogue()
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
        m_typewriter.StopShowingText();
        m_currentDialogue = null;
        OnDialogueInterrupted?.Invoke();
    }
}