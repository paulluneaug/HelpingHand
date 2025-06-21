using System;

using UnityEngine;

[Serializable]
public class ButtonAudioEventController : AudioEventController
{
    [SerializeField] private ButtonInputEvent m_buttonEvent;

    [SerializeField] private WwiseInputEventPair m_buttonUpEvents;
    [SerializeField] private WwiseInputEventPair m_buttonDownEvents;


    public override void Init(GameObject defaultSource)
    {
        base.Init(defaultSource);
        m_buttonEvent.ButtonDownEvent.OnEventRaised += OnButtonDownRaised;
        m_buttonEvent.ButtonUpEvent.OnEventRaised += OnButtonUpRaised;
    }

    public override void Dispose()
    {
        m_buttonEvent.ButtonDownEvent.OnEventRaised -= OnButtonDownRaised;
        m_buttonEvent.ButtonUpEvent.OnEventRaised -= OnButtonUpRaised;
    }

    private void OnButtonDownRaised()
    {
        _ = m_buttonDownEvents.PostEvent(m_buttonEvent.ButtonUpEvent.IsActive, m_source);
    }

    private void OnButtonUpRaised()
    {
        _ = m_buttonUpEvents.PostEvent(m_buttonEvent.ButtonUpEvent.IsActive, m_source);
    }
}