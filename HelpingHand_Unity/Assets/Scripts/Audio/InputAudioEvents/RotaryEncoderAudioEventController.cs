using System;

using UnityEngine;

[Serializable]
public class RotaryEncoderAudioEventController : AudioEventController
{
    [SerializeField] private RotaryEncoderInputEvent m_rotaryEncoderEvent;

    [SerializeField] private WwiseInputEventPair m_stepLeftEvents;
    [SerializeField] private WwiseInputEventPair m_stepRightEvents;


    public override void Init(GameObject defaultSource)
    {
        base.Init(defaultSource);
        m_rotaryEncoderEvent.StepLeftEvent.OnEventRaised += OnStepLeftRaised;
        m_rotaryEncoderEvent.StepRightEvent.OnEventRaised += OnStepRightRaised;
    }

    public override void Dispose()
    {
        m_rotaryEncoderEvent.StepLeftEvent.OnEventRaised -= OnStepLeftRaised;
        m_rotaryEncoderEvent.StepRightEvent.OnEventRaised -= OnStepRightRaised;
    }

    private void OnStepLeftRaised()
    {
        _ = m_stepLeftEvents.PostEvent(m_rotaryEncoderEvent.StepLeftEvent.IsActive, m_source);
    }

    private void OnStepRightRaised()
    {
        _ = m_stepRightEvents.PostEvent(m_rotaryEncoderEvent.StepRightEvent.IsActive, m_source);
    }
}