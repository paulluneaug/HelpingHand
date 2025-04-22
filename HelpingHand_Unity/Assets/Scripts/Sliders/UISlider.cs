using System;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;

public class UISlider : MonoBehaviour
{
    public bool IsPointerDown => m_pointerDown;

    public event Action<float> OnValueChanged;
    public event Action<bool> OnPointerDown;

    [SerializeField] private Slider m_slider;
    [SerializeField] private PointerEventDispatcher m_pointerEventDispatcher;
    [SerializeField, Disable] private bool m_pointerDown;

    private void Awake()
    {
        m_slider.onValueChanged.AddListener(OnSliderValueChanged);

        m_pointerEventDispatcher.OnPointerDownChanged += OnPointerDownChanged;
    }

    private void OnDestroy()
    {
        m_slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        m_pointerEventDispatcher.OnPointerDownChanged -= OnPointerDownChanged;
    }

    public void SetSliderValue(float value)
    {
        m_slider.value = value;
    }

    private void OnSliderValueChanged(float value)
    {
        OnValueChanged?.Invoke(value);
    }

    public void OnPointerDownChanged(bool pointerDown)
    {
        m_pointerDown = pointerDown;
        OnPointerDown?.Invoke(pointerDown);
    }
}
