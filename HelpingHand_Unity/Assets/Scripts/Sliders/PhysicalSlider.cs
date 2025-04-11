using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicalSlider : MonoBehaviour
{
    public event Action<float> OnValueChanged;

    [SerializeField] private InputActionReference m_sliderAction;


    private void Awake()
    {
        m_sliderAction.action.performed += OnSliderValueChanged;
    }

    public void SetSliderValue(float value)
    {
        // TODO
    }

    private void OnSliderValueChanged(InputAction.CallbackContext context)
    {
        OnValueChanged?.Invoke(context.action.ReadValue<float>());
    }
}
