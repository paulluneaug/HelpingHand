using System;

using UnityEngine;

[Serializable]
public class ToggleAudioEventController : AudioEventController
{
    [SerializeField] private ToggleInputEvent m_toggleEvent;

    [SerializeField] private WwiseInputEventPair m_toggleUpEvents;
    [SerializeField] private WwiseInputEventPair m_toggleDownEvents;


    public override void Init(GameObject defaultSource)
    {
        base.Init(defaultSource);
        m_toggleEvent.OnEventRaised += OnToggleRaised;
    }

    public override void Dispose()
    {
        m_toggleEvent.OnEventRaised -= OnToggleRaised;
    }

    private void OnToggleRaised()
    {
        WwiseInputEventPair eventPairToPost = m_toggleEvent.Value ? m_toggleDownEvents : m_toggleUpEvents;
        _ = eventPairToPost.PostEvent(m_toggleEvent.IsActive, m_source);
    }
}