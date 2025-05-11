using System;
using System.Collections;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Extensions;

[Serializable]
public class SliderInputTrigger : InputTrigger
{
    [SerializeField]
    private FloatVariable m_sliderVariable;

    [SerializeField, MinMaxSlider(0, 1, true)]
    private Vector2 m_targetRange;

    [SerializeField]
    private float m_timeToTrigger = 0.2f;

    [SerializeField]
    private bool m_isImmediate = false;

    private Coroutine m_triggerCoroutine;

    public override bool IsRaised => m_isImmediate ? m_isRaised : TestValue();

    private bool m_wasRaised;
    
    public override void Initialize()
    {
        base.Initialize();
        SetActive(true);
        m_wasRaised = false;
    }

    protected override void Activate()
    {
        m_sliderVariable.AddListener(OnSliderValueChanged);
    }

    protected override void Deactivate()
    {
        m_sliderVariable.RemoveListener(OnSliderValueChanged);
    }

    private bool TestValue()
    {
        return m_sliderVariable.Value.Between(m_targetRange.x, m_targetRange.y);
    }

    private void OnSliderValueChanged(float value)
    {
        // We want to start the trigger coroutine when the trigger is not raised, the coroutine is not already running, and the test is true
        if (!m_isRaised && m_triggerCoroutine == null && TestValue())
        {
            m_triggerCoroutine = DialogueManager.Instance.StartCoroutine(TriggerCoroutine());
        } 
        
        // If the trigger was raised previously && the new value steps out of the range values, trigger the event
        if (m_wasRaised && !TestValue())
        {
            Debug.Log($"{m_sliderVariable.name} Not raised anymore");
            RaiseTriggeredEvent();
        }

        m_wasRaised = IsRaised;
    }

    private IEnumerator TriggerCoroutine()
    {
        float counter = 0;
        while (counter < m_timeToTrigger)
        {
            // Slider's value exit the target range => stop the coroutine
            if (!TestValue())
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
        RaiseTriggeredEvent();

        DialogueManager.Instance.StartCoroutine(ReactivateCoroutine());
    }
}