using System;

[Serializable]
public class AxisInputActionTrigger : InputActionTrigger<FloatInputEvent>
{
    protected override void UpdateEvent()
    {
        m_linkedEvent.Value = m_action.action.ReadValue<float>();
    }
}
