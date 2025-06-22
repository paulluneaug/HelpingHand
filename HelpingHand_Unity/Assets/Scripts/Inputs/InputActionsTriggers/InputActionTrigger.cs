using System;

using Events;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public abstract class InputActionTrigger<TEvent> : IDisposable
    where TEvent : BaseGameEvent
{
    [SerializeField] protected InputActionReference m_action;
    [SerializeField] protected TEvent m_linkedEvent;
    [SerializeField, EnumToggleButtons] private TriggerBehaviour m_behaviour = TriggerBehaviour.OnActionPerformed; 


    public void Initialize()
    {
        if (m_behaviour != TriggerBehaviour.OnActionPerformed)
        {
            return;
        }
        m_action.action.performed += OnActionPerformed;
    }

    public void Dispose()
    {
        if (m_behaviour != TriggerBehaviour.OnActionPerformed)
        {
            return;
        }
        m_action.action.performed -= OnActionPerformed;
    }

    public void Update()
    {
        if (m_behaviour != TriggerBehaviour.OnUpdate) 
        {
            return;
        }
        UpdateEvent();
    }

    protected void OnActionPerformed(InputAction.CallbackContext context)
    {
        UpdateEvent();
    }

    protected abstract void UpdateEvent();
}
