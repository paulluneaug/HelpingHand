using UnityEngine;

using UnityUtility.CustomAttributes;

public class MasterSlider : MonoBehaviour
{
    public float Value
    {
        get => m_value;
        set
        {
            m_value = value;
            m_uiSlider.SetSliderValue(value);
            m_physicalSlider.SetSliderValue(value);
        }
    }


    [SerializeField] private UISlider m_uiSlider;
    [SerializeField] private PhysicalSlider m_physicalSlider;

    [SerializeField, Disable] private float m_value;

    private void Awake()
    {
        m_uiSlider.OnValueChanged += OnUISliderValueChanged;
        m_physicalSlider.OnValueChanged += OnPhysicalSliderValueChanged;
    }

    private void OnPhysicalSliderValueChanged(float newValue)
    {
        m_value = newValue;
        m_uiSlider.SetSliderValue(newValue);
    }

    private void OnUISliderValueChanged(float newValue)
    {
        m_value = newValue;
        m_physicalSlider.SetSliderValue(newValue);
    }
}
