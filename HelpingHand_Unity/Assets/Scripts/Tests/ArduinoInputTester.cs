using UnityEngine;
using UnityEngine.InputSystem;

public class ArduinoInputTester : MonoBehaviour
{
    [SerializeField] private InputActionReference m_arduinoInput;

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
    }
}
