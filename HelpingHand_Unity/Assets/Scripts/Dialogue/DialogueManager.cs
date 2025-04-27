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

    private Dialogue[] m_dialogues;
    private readonly SortedSet<Dialogue> m_dialogQueue = new(Comparer<Dialogue>.Create((d1, d2) => d1.Priority.CompareTo(d2.Priority)));

    protected override void Start()
    {
        base.Start();
        m_dialogues = Resources.LoadAll<Dialogue>("Dialogues");
        
        foreach (Dialogue dialogue in m_dialogues)
        {
            dialogue.Initialize();
        }
    }

    private void Update()
    {
        if (m_typewriter.isShowingText)
        {
            return;
        }

        if (m_dialogQueue.Count == 0)
        {
            return;
        }

        WriteDialog();
    }

    private void WriteDialog()
    {
        Dialogue dialogue = m_dialogQueue.Min;
        m_dialogQueue.Remove(dialogue);
        m_typewriter.ShowText(dialogue.Content);
    }

    public void PlayDialog(Dialogue dialogue)
    {
        m_dialogQueue.Add(dialogue);
    }
}