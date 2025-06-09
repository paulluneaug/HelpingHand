using System;

using UnityEngine;

[Serializable]
public class RotaryEncoderInputTrigger : InputTrigger
{
    [SerializeField]
    private RotaryEncoderInputEvent m_event;

    public override bool IsRaised => true;

    public override void Initialize()
    {
        base.Initialize();

        ArmTrigger();
    }

    protected override void ArmTrigger()
    {
        m_event.AddIndexListener(RaiseTriggeredEvent);
        m_event.AddStepLeftListener(RaiseTriggeredEvent);
        m_event.AddStepRightListener(RaiseTriggeredEvent);
    }

    protected override void DisarmTrigger()
    {
        m_event.RemoveIndexListener(RaiseTriggeredEvent);
        m_event.RemoveStepLeftListener(RaiseTriggeredEvent);
        m_event.RemoveStepRightListener(RaiseTriggeredEvent);
    }
}
