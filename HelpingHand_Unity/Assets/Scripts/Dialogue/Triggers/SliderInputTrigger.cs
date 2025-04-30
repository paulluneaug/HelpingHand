using System;
using System.Collections;

using Events;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Extensions;

[Serializable]
public class SliderInputTrigger : InputTrigger
{
    [SerializeField]
    private FloatGameEvent m_sliderEvent;

    [SerializeField, MinMaxSlider(0, 1, true)]
    private Vector2 m_targetRange;

    [SerializeField]
    private float m_timeToTrigger = 0.2f;

    [SerializeField]
    private bool m_isImmediate = true;

    private float m_currentSliderValue;
    private Coroutine m_triggerCoroutine;

    public override bool IsRaised => m_isImmediate ? m_isRaised : m_currentSliderValue.Between(m_targetRange.x, m_targetRange.y);

    public override void Initialize()
    {
        base.Initialize();
        m_currentSliderValue = 0; // Attention c'est pas forcément vrai, il faudrait récupérer la valeur actuelle du slider
        SetActive(true);
    }

    protected override void Activate()
    {
        m_sliderEvent.AddListener(OnSliderValueChanged);
    }

    protected override void Deactivate()
    {
        m_sliderEvent.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        m_currentSliderValue = value;

        if (value.Between(m_targetRange.x, m_targetRange.y) && m_triggerCoroutine == null)
        {
            m_triggerCoroutine = DialogueManager.Instance.StartCoroutine(TriggerCoroutine());
        }
    }

    private IEnumerator TriggerCoroutine()
    {
        float counter = 0;
        while (counter < m_timeToTrigger)
        {
            // Slider's value exit the target range => stop the coroutine
            if (!m_currentSliderValue.Between(m_targetRange.x, m_targetRange.y))
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

        DialogueManager.Instance.StartCoroutine(ReactivateCoroutine());
    }
}