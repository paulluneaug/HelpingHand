using UnityEngine;
using UnityEngine.UI;

public class VirtualFader : VirtualInput<float>
{
    [SerializeField] private Slider m_slider;

    private void Awake()
    {
        m_slider.onValueChanged.AddListener(OnSliderValueChanged);
        SetValueWithoutNotify(m_slider.value);

    }

    private void OnSliderValueChanged(float value)
    {
        SetValue(value);
    }
}
