using UnityEngine;

[System.Serializable]
public class ConditionDialogueCount : ConditionBase
{
    [SerializeField]
    private int m_countNeeded = 0;

    [SerializeField]
    private bool m_strictlyEquals = false;

    [SerializeField]
    private DialogueNode m_dialogue;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override bool Test()
    {
        return m_strictlyEquals ? m_dialogue.ReadCount == m_countNeeded : m_dialogue.ReadCount >= m_countNeeded;
    }
}