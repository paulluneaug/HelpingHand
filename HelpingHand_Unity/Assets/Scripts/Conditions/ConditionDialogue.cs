using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ConditionDialogue : ConditionBase
{
    [SerializeField]
    private Dialogue m_dialogue;

    public override bool Test()
    {
        return m_dialogue.HasBeenRead.Value;
    }

    public override void Initialize()
    {
        base.Initialize();
        m_dialogue.HasBeenRead.OnValueChanged -= OnValueChanged;
        m_dialogue.HasBeenRead.OnValueChanged += OnValueChanged;
    }

    private void OnValueChanged(bool value)
    {
        RaiseOnPreconditionUpdated();
    }
}