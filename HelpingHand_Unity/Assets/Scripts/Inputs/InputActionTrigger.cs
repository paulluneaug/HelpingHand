using System;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class InputActionTrigger<T> : IDisposable
    where T : struct
{
    private enum TriggerBehaviour
    {
        OnUpdate,
        OnActionPerformed,
    }

    [SerializeField] private InputActionReference m_action;
    [SerializeField] private BaseVariable<T> m_linkedVariable;

    [SerializeField, EnumToggleButtons] private TriggerBehaviour m_behaviour;

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
        m_linkedVariable.Value = GetActionValue();
    }

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        m_linkedVariable.Value = GetActionValue();
    }

    private T GetActionValue()
    {
        return m_action.action.ReadValue<T>();
    }
}
