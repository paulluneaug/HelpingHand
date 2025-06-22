using System;

using UnityEngine;

using UnityUtility.Extensions;

[Serializable]
public class InputActionTriggersManager : MonoBehaviour
{
    [SerializeField] private ButtonInputActionTrigger[] m_buttonsTriggers;
    [SerializeField] private ToggleInputActionTrigger[] m_toggleTriggers;
    [SerializeField] private AxisInputActionTrigger[] m_axisTriggers;
    [SerializeField] private RotaryEncoderInputActionTrigger[] m_rotaryTriggers;
    [SerializeField] private JoystickInputActionTrigger[] m_joystickTriggers;

    private void Awake()
    {
        m_buttonsTriggers.ForEach(trigger => trigger.Initialize());
        m_toggleTriggers.ForEach(trigger => trigger.Initialize());
        m_axisTriggers.ForEach(trigger => trigger.Initialize());
        m_rotaryTriggers.ForEach(trigger => trigger.Initialize());
        m_joystickTriggers.ForEach(trigger => trigger.Initialize());
    }

    private void Update()
    {
        m_buttonsTriggers.ForEach(trigger => trigger.Update());
        m_toggleTriggers.ForEach(trigger => trigger.Update());
        m_axisTriggers.ForEach(trigger => trigger.Update());
        m_rotaryTriggers.ForEach(trigger => trigger.Update());
        m_joystickTriggers.ForEach(trigger => trigger.Update());
    }

    private void OnDestroy()  
    {
        m_buttonsTriggers.ForEach(trigger => trigger.Dispose());
        m_toggleTriggers.ForEach(trigger => trigger.Dispose());
        m_axisTriggers.ForEach(trigger => trigger.Dispose());
        m_rotaryTriggers.ForEach(trigger => trigger.Dispose());
        m_joystickTriggers.ForEach(trigger => trigger.Dispose());
    }
}
