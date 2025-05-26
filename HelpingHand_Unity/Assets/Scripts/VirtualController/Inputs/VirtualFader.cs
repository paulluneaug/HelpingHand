using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VirtualFader : VirtualInput<float>
{
    protected override BaseVariable<float> InputEvent => m_inputEvent;

    [FormerlySerializedAs("m_linkedVariable")]
    [SerializeField]
    private FaderInputEvent m_inputEvent;

    [SerializeField]
    private Slider m_slider;

    [SerializeField]
    [Range(0, 1)]
    private float m_startValue;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        m_slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void Start()
    {
        SetValueWithoutNotify(m_startValue);
        m_slider.SetValueWithoutNotify(m_startValue);
    }

    private void OnSliderValueChanged(float value)
    {
        SetValue(value);
    }
}
