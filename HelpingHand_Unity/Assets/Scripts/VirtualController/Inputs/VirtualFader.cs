using UnityEngine;
using UnityEngine.UI;

public class VirtualFader : VirtualInput<float>
{
    protected override BaseVariable<float> LinkedVariable => m_linkedVariable;
    
    [SerializeField]
    private FaderInputEvent m_linkedVariable;
    
    [SerializeField] 
    private Slider m_slider;

    [SerializeField]
    [Range(0, 1)]
    private float m_startValue;

    private void Awake()
    {
        m_slider.onValueChanged.AddListener(OnSliderValueChanged);
        SetValueWithoutNotify(m_startValue);
        m_slider.SetValueWithoutNotify(m_startValue);
    }

    private void OnSliderValueChanged(float value)
    {
        SetValue(value);
    }
}
