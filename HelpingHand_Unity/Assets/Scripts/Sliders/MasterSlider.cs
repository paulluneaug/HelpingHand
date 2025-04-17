using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Filters;
using UnityUtility.MathU;

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

    public float Speed => m_speed;
    public float MovementDirection => m_movementDirection;


    [SerializeField] private UISlider m_uiSlider;
    [SerializeField] private PhysicalSlider m_physicalSlider;

    [SerializeField] private OneEuroFilterSettings m_speedFilterSettings;

    [SerializeField, Disable] private float m_value;
    [SerializeField, Disable] private float m_speed;
    [SerializeField, Disable] private float m_movementDirection;

    [NonSerialized] private float m_previousValue;
    [NonSerialized] private OneEuroFilter m_speedFilter;

    private void Awake()
    {
        m_uiSlider.OnValueChanged += OnUISliderValueChanged;
        m_physicalSlider.OnValueChanged += OnPhysicalSliderValueChanged;

        m_speedFilter = new OneEuroFilter(m_speedFilterSettings.MinCutoff, m_speedFilterSettings.Beta);
        m_speed = m_speedFilter.Filter(0.0f, 0.0f);

    }

    private void Update()
    {
        UpdateSliderSpeed(Time.deltaTime);
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

    private void UpdateSliderSpeed(float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return;
        }

        float deltaPosition = m_value - m_previousValue;
        float speed = deltaPosition / deltaTime;

        float newSpeed = MathUf.Round(m_speedFilter.Filter(speed, deltaTime), 3);
        m_speed = MathUf.Abs(newSpeed);
        m_movementDirection = MathUf.Sign(newSpeed);

        m_previousValue = m_value;
    }

}
