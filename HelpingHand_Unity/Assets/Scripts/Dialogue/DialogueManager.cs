using System.Collections.Generic;

using Febucci.UI;

using TMPro;

using UnityEngine;

using UnityUtility.Singletons;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
    [SerializeField]
    private TMP_Text m_uiText;

    [SerializeField]
    private TypewriterByCharacter m_typewriter;

    public bool IsShowingText => m_currentDialogue != null;
    public bool CanBeInterrupted => !IsShowingText || m_currentDialogue.CanBeInterrupted;
    public Dialogue CurrentDialogue => m_currentDialogue;

    private Dialogue[] m_dialogues;

    private readonly SortedSet<Dialogue> m_dialogQueue = new(Comparer<Dialogue>.Create((d1, d2) => d1.Priority.CompareTo(d2.Priority)));
    private Dialogue m_currentDialogue;
    private Dialogue m_interruptedDialogue;

    protected override void Start()
    {
        base.Start();
        m_dialogues = Resources.LoadAll<Dialogue>("Dialogues");

        foreach (Dialogue dialogue in m_dialogues)
        {
            dialogue.Initialize();
        }

        m_typewriter.onTextShowed.AddListener(OnTextShowed);
    }

    private void Update()
    {
        if (IsShowingText)
        {
            return;
        }
    
        if (m_dialogQueue.Count == 0)
        {
            return;
        }
    
        Dialogue dialogue = m_dialogQueue.Min;
        m_dialogQueue.Clear();
        dialogue.HasBeenRead.Value = true;
        ShowDialogue(dialogue);
    }

    private void OnTextShowed()
    {
        Debug.Log($"[OnTextShowed] <color=green>[{m_currentDialogue}]</color>");
        m_currentDialogue = null;
    }

    public void PlayDialog(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            return;
        }

        m_dialogQueue.Add(dialogue);
    }

    private void ShowDialogue(Dialogue dialogue)
    {
        Debug.Log($"[ShowDialogue] <color=green>[{dialogue.name}]</color>");
        m_currentDialogue = dialogue;
        m_typewriter.ShowText(m_currentDialogue.Content);
    }

    public void Interrupt()
    {
        m_interruptedDialogue = m_currentDialogue;
        if (m_interruptedDialogue != null)
        {
            Debug.Log($"{m_interruptedDialogue.name} interrupted");
            m_typewriter.StopShowingText();
            m_currentDialogue = null;
        }
    }
}