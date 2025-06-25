using System;

using UnityEngine;

[Serializable]
public class JoystickInputActionTrigger : InputActionTrigger<JoystickInputEvent>
{
    protected override void UpdateEvent()
    {
        m_linkedEvent.Value = m_action.action.ReadValue<Vector2>();
    }
}
