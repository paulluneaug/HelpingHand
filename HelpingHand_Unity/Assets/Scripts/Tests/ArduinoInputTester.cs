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
    }

    private void OnArduinoInputPerformed(InputAction.CallbackContext context)
    {
        Debug.LogWarning($"[{Time.frameCount}] Arduino Action Peformed");
    }

    // Update is called once per frame
    private void Update()
    {
        bool arduinoInput = m_arduinoInput.action.IsPressed();
        Debug.Log($"[{Time.frameCount}] Arduino Action pressed : {arduinoInput}");

        m_testedActions.ForEach( action => action.Update() );
    }
}
