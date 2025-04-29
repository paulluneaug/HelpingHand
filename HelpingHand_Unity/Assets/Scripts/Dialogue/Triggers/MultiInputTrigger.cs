using System;

using UnityEngine;

[Serializable]
public class MultiInputTrigger : InputTrigger
{
    [SerializeField]
    private InputTrigger[] m_triggers;

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
            inputTrigger.RaiseTriggerEvent -= OnTriggerRaised;
            inputTrigger.RaiseTriggerEvent += OnTriggerRaised;
        }
    }

    protected override void Deactivate()
    {
        foreach (InputTrigger inputTrigger in m_triggers)
        {
            inputTrigger.RaiseTriggerEvent -= OnTriggerRaised;
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
        RaiseTrigger();
        DialogueManager.Instance.StartCoroutine(ReactivateCoroutine());
    }
}