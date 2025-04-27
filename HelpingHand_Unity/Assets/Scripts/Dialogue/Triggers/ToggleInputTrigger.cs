using System;
using System.Collections;

using Events;

using UnityEngine;

[Serializable]
public class ToggleInputTrigger : InputTrigger
{
    [SerializeField]
    private BoolGameEvent m_toggleEvent;

    [SerializeField]
    private bool m_triggerIfTrue = true;
    
    [SerializeField]
    private float m_timeToTrigger = 0.05f;

    public override bool IsRaised => m_currentValue;

    private bool m_currentValue;
    private Coroutine m_triggerCoroutine;

    public override void Initialize()
    {
        base.Initialize();
        m_currentValue = false; // Attention c'est pas forcément vrai, il faudrait récupérer la valeur actuelle du toggle
        SetActive(true);
    }

    protected override void Activate()
    {
        m_toggleEvent.AddListener(OnToggleValueChanged);
    }

    protected override void Deactivate()
    {
        m_toggleEvent.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        m_currentValue = isOn;

        if (m_triggerIfTrue == m_currentValue && m_triggerCoroutine == null)
        {
            m_triggerCoroutine = DialogueManager.Instance.StartCoroutine(TriggerCoroutine());
        }
    }

    private IEnumerator TriggerCoroutine()
    {
        float counter = 0;
        while (counter < m_timeToTrigger)
        {
            // Toggle's value is not the right value => stop the coroutine
            if (m_triggerIfTrue != m_currentValue)
            {
                m_triggerCoroutine = null;
                yield break;
            }

            counter += Time.deltaTime;
            yield return null;
        }

        // timer has been reached
        m_isRaised = true;
        m_triggerCoroutine = null;
        RaiseTrigger();
    }
}