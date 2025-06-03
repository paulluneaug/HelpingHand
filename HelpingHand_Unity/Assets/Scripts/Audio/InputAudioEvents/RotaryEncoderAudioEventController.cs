using System;

using UnityEngine;

[Serializable]
public class RotaryEncoderAudioEventController : IDisposable
{
    [SerializeField] private RotaryEncoderInputEvent m_rotaryEncoderEvent;

    [SerializeField] private WwiseInputEventPair m_stepLeftEvents;
    [SerializeField] private WwiseInputEventPair m_stepRightEvents;

    [Tooltip("Can be null")]
    [SerializeField] private GameObject m_audioSource;


    public void Init()
    {
        m_rotaryEncoderEvent.StepLeftEvent.OnEventRaised += OnStepLeftRaised;
        m_rotaryEncoderEvent.StepRightEvent.OnEventRaised += OnStepRightRaised;
    }

    public void Dispose()
    {
        m_rotaryEncoderEvent.StepLeftEvent.OnEventRaised -= OnStepLeftRaised;
        m_rotaryEncoderEvent.StepRightEvent.OnEventRaised -= OnStepRightRaised;
    }

    private void OnStepLeftRaised()
    {
        _ = m_stepLeftEvents.PostEvent(m_rotaryEncoderEvent.StepLeftEvent.IsActive, m_audioSource);
    }

    private void OnStepRightRaised()
    {
        _ = m_stepRightEvents.PostEvent(m_rotaryEncoderEvent.StepRightEvent.IsActive, m_audioSource);
    }
}