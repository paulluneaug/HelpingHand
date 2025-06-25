using System;

[Serializable]
public class ButtonInputActionTrigger : InputActionTrigger<ButtonInputEvent>
{
    protected override void UpdateEvent()
    {
        if (m_action.action.IsPressed())
        {
            m_linkedEvent.RaiseDown();
            return;
        }
        m_linkedEvent.RaiseUp();
    }
}
