using System;

using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    public event Action<float> OnValueChanged;

    [SerializeField] private Slider m_slider;

    private void Awake()
    {
        m_slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnDestroy()
    {
        m_slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    public void SetSliderValue(float value)
    {
        m_slider.value = value;
    }

    private void OnSliderValueChanged(float value) 
    {
        OnValueChanged?.Invoke(value);
    }
}
