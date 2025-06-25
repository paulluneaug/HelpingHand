using System;
using System.Runtime.Remoting;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.Extensions;

[Serializable]
public class InputActionTriggersManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_actionAsset;
    [SerializeField] private PlayerInput m_playerInput;
    [SerializeField] private ButtonInputActionTrigger[] m_buttonsTriggers;
    [SerializeField] private ToggleInputActionTrigger[] m_toggleTriggers;
    [SerializeField] private AxisInputActionTrigger[] m_axisTriggers;
    [SerializeField] private RotaryEncoderInputActionTrigger[] m_rotaryTriggers;
    [SerializeField] private JoystickInputActionTrigger[] m_joystickTriggers;

    private void Awake()
    {
        m_actionAsset.Enable();
        //m_playerInput.SwitchCurrentControlScheme("Arduino", m_actionAsset.devices.Value.ToArray());
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

    [Button]
    private void SyncDevice()
    {
        
        foreach (var item in InputSystem.devices)
        {
            bool result = InputSystem.TrySyncDevice(item);
        }
    }
    [Button]
    private void DisableDevice()
    {
        foreach (var item in InputSystem.devices)
        {
            InputSystem.DisableDevice(item);
        }
    }
    [Button]
    private void EnableDevice()
    {
        foreach (var item in InputSystem.devices)
        {
            InputSystem.EnableDevice(item);
        }
    }


}
