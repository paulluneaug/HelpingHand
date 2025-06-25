using System;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class RotaryEncoderInputActionTrigger
{
    [SerializeField] protected InputActionReference m_actionLeft;
    [SerializeField] protected InputActionReference m_actionRight;
    [SerializeField] protected RotaryEncoderInputEvent m_linkedEvent;
    [SerializeField, EnumToggleButtons] private TriggerBehaviour m_behaviour = TriggerBehaviour.OnActionPerformed;


    public void Initialize()
    {
        if (m_behaviour != TriggerBehaviour.OnActionPerformed)
        {
            return;
        }

        m_actionLeft.action.performed += OnActionLeftPerformed;
        m_actionRight.action.performed += OnActionRightPerformed;
    }


    public void Dispose()
    {
        if (m_behaviour != TriggerBehaviour.OnActionPerformed)
        {
            return;
        }

        m_actionLeft.action.performed -= OnActionLeftPerformed;
        m_actionRight.action.performed -= OnActionRightPerformed;
    }

    public void Update()
    {
        if (m_behaviour != TriggerBehaviour.OnUpdate)
        {
            return;
        }
        UpdateLeftEvent();
        UpdateRightEvent();
    }

    private void OnActionLeftPerformed(InputAction.CallbackContext context)
    {
        UpdateLeftEvent();
    }

    private void OnActionRightPerformed(InputAction.CallbackContext context)
    {
        UpdateRightEvent();
    }

    private void UpdateLeftEvent()
    {
        if (m_actionLeft.action.IsPressed())
        {
            m_linkedEvent.RaiseStepLeft();
        }
    }

    private void UpdateRightEvent()
    {
        if (m_actionRight.action.IsPressed())
        {
            m_linkedEvent.RaiseStepRight();
        }
    }
}
