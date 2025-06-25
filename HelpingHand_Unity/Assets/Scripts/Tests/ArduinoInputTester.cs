using System;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;

public class ArduinoInputTester : MonoBehaviour
{
    [Serializable]
    private class ActionValueTester
    {
        [SerializeField] private InputActionReference m_action;
        [SerializeField, Disable] private float m_value;

        public void Update()
        {
            m_value = m_action.action.ReadValue<float>();
        }
    }

    [SerializeField] private InputActionReference m_arduinoInput;

    [SerializeField] private ActionValueTester[] m_testedActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        m_arduinoInput.action.performed += OnArduinoInputPerformed;
        m_arduinoInput.action.started += OnArduinoInputStarted;
    }

    private void OnArduinoInputStarted(InputAction.CallbackContext context)
    {
    }

    private void OnArduinoInputPerformed(InputAction.CallbackContext context)
    {
        //Debug.LogWarning($"[{Time.frameCount}] Arduino Action Peformed");
    }

    // Update is called once per frame
    private void Update()
    {
        bool arduinoInput = m_arduinoInput.action.IsPressed();

        m_testedActions.ForEach(action => action.Update());
    }
}
