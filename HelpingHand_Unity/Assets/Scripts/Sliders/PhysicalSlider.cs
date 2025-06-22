using System;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.CustomAttributes;

public class PhysicalSlider : MonoBehaviour
{
    public bool FingerDown => m_fingerDown;

    public event Action<float> OnValueChanged;
    public event Action<bool> OnPointerDownChanged;

    [SerializeField] private InputActionReference m_sliderAction;
    [SerializeField] private InputActionReference m_fingerDownAction;

    [SerializeField, Disable] private float m_sliderValue;
    [SerializeField, Disable] private bool m_fingerDown;


    private void Awake()
    {
        m_sliderAction.action.performed += OnSliderValueChanged;
        m_fingerDownAction.action.performed += OnFingerDownActionPerformed;
    }

    private void OnDestroy()
    {
        m_sliderAction.action.performed -= OnSliderValueChanged;
        m_fingerDownAction.action.performed -= OnFingerDownActionPerformed;
    }

    public void SetSliderValue(float value)
    {
        // TODO
    }

    private void OnSliderValueChanged(InputAction.CallbackContext context)
    {
        m_sliderValue = context.action.ReadValue<float>();
        OnValueChanged?.Invoke(m_sliderValue);
    }

    private void OnFingerDownActionPerformed(InputAction.CallbackContext context)
    {
        m_fingerDown = context.action.ReadValue<bool>();
        OnPointerDownChanged?.Invoke(m_fingerDown);
    }
}
