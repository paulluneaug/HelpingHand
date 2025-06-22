using UnityEngine;

[System.Serializable]
public class ConditionDialogue : ConditionBase
{
    [SerializeField]
    private DialogueNode m_dialogue;

    [SerializeField]
    private bool m_isRead = true;

    public override void Initialize()
    {
        base.Initialize();
        m_dialogue.HasBeenRead.OnValueChanged -= OnValueChanged;
        m_dialogue.HasBeenRead.OnValueChanged += OnValueChanged;
    }

    public override void Dispose()
    {
        base.Dispose();
        m_dialogue.HasBeenRead.OnValueChanged -= OnValueChanged;
    }

    public override bool Test()
    {
        return m_isRead == m_dialogue.HasBeenRead.Value;
    }

    private void OnValueChanged(bool value)
    {
        RaiseOnPreconditionUpdated();
    }
}