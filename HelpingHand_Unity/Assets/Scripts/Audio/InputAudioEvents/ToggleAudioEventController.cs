using System;

using UnityEngine;

[Serializable]
public class ToggleAudioEventController : IDisposable
{
    [SerializeField] private ToggleInputEvent m_toggleEvent;

    [SerializeField] private WwiseInputEventPair m_toggleUpEvents;
    [SerializeField] private WwiseInputEventPair m_toggleDownEvents;

    [Tooltip("Can be null")]
    [SerializeField] private GameObject m_audioSource;


    public void Init()
    {
        m_toggleEvent.OnEventRaised += OnToggleRaised;
    }

    public void Dispose()
    {
        m_toggleEvent.OnEventRaised -= OnToggleRaised;
    }

    private void OnToggleRaised()
    {
        WwiseInputEventPair eventPairToPost = m_toggleEvent.Value ? m_toggleDownEvents : m_toggleUpEvents;
        _ = eventPairToPost.PostEvent(m_toggleEvent.IsActive, m_audioSource);
    }
}