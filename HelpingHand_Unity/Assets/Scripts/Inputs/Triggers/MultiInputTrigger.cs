using System;

using UnityEngine;

[Serializable]
public class MultiInputTrigger : InputTrigger
{
    [SerializeField]
    private InputTrigger[] m_triggers = Array.Empty<InputTrigger>();

    public override void Initialize()
    {
        base.Initialize();
        SetActive(true);
        foreach (InputTrigger inputTrigger in m_triggers)
        {
            inputTrigger.Initialize();
        }
    }

    protected override void Activate()
    {
        foreach (InputTrigger inputTrigger in m_triggers)
        {
            inputTrigger.OnTriggered -= OnTriggerRaised;
            inputTrigger.OnTriggered += OnTriggerRaised;
        }
    }

    protected override void Deactivate()
    {
        foreach (InputTrigger inputTrigger in m_triggers)
        {
            inputTrigger.OnTriggered -= OnTriggerRaised;
        }
    }

    private void OnTriggerRaised()
    {
        foreach (InputTrigger inputTrigger in m_triggers)
        {
            if (!inputTrigger.IsRaised)
            {
                return;
            }
        }

        m_isRaised = true;
        SetActive(false);
        RaiseTriggeredEvent();
        DialogueManager.Instance.StartCoroutine(ReactivateCoroutine());
    }
}