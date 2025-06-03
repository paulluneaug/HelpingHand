using System;

using UnityEngine;

[Serializable]
public class ButtonAudioEventController : IDisposable
{
    [SerializeField] private ButtonInputEvent m_buttonEvent;

    [SerializeField] private WwiseInputEventPair m_buttonUpEvents;
    [SerializeField] private WwiseInputEventPair m_buttonDownEvents;

    [Tooltip("Can be null")]
    [SerializeField] private GameObject m_audioSource;


    public void Init()
    {
        m_buttonEvent.ButtonDownEvent.OnEventRaised += OnButtonDownRaised;
        m_buttonEvent.ButtonUpEvent.OnEventRaised += OnButtonUpRaised;
    }

    public void Dispose()
    {
        m_buttonEvent.ButtonDownEvent.OnEventRaised -= OnButtonDownRaised;
        m_buttonEvent.ButtonUpEvent.OnEventRaised -= OnButtonUpRaised;
    }

    private void OnButtonDownRaised()
    {
        _ = m_buttonDownEvents.PostEvent(m_buttonEvent.ButtonUpEvent.IsActive, m_audioSource);
    }

    private void OnButtonUpRaised()
    {
        _ = m_buttonUpEvents.PostEvent(m_buttonEvent.ButtonUpEvent.IsActive, m_audioSource);
    }
}