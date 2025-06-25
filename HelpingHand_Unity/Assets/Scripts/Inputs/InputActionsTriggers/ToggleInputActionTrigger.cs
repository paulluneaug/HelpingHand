using System;

[Serializable]
public class ToggleInputActionTrigger : InputActionTrigger<ToggleInputEvent>
{
    protected override void UpdateEvent()
    {
        m_linkedEvent.Value = m_action.action.IsPressed();
    }
}
