using System;
using System.Collections;

using UnityEngine;

[Serializable]
public class ButtonInputTrigger : InputTrigger
{
    [SerializeField]
    private ButtonInputEvent m_buttonEvent;

    [SerializeField]
    private float m_timeToTrigger = 0.05f;

    private Coroutine m_triggerCoroutine;
    private bool m_isButtonPressed;

    public override void Initialize()
    {
        base.Initialize();
        SetActive(true);
    }

    protected override void Activate()
    {
        m_buttonEvent.AddDownListener(OnButtonDown);
        m_buttonEvent.AddUpListener(OnButtonUp);
    }

    protected override void Deactivate()
    {
        m_buttonEvent.RemoveDownListener(OnButtonDown);
        m_buttonEvent.RemoveUpListener(OnButtonUp);
    }

    private void OnButtonDown()
    {
        m_isButtonPressed = true;

        if (m_triggerCoroutine == null)
        {
            _ = DialogueManager.Instance.StartCoroutine(TriggerCoroutine());
        }
    }

    private void OnButtonUp()
    {
        m_isButtonPressed = false;
    }

    private IEnumerator TriggerCoroutine()
    {
        float counter = 0;
        while (counter < m_timeToTrigger)
        {
            // If we stop pressing the button => stop the coroutine
            if (!m_isButtonPressed)
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
        SetActive(false);
        RaiseTrigger();

        _ = DialogueManager.Instance.StartCoroutine(ReactivateCoroutine());
    }
}
