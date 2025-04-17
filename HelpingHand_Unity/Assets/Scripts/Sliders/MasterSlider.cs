using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Filters;
using UnityUtility.MathU;

public class MasterSlider : MonoBehaviour
{
    public const float MIN_VALUE = 0.0f;
    public const float MAX_VALUE = 1.0f;

    public float Value
    {
        get => m_value;
        set
        {
            m_value = value;
            m_uiSlider.SetSliderValue(value);
            m_physicalSlider.SetSliderValue(value);
            OnSliderValueChanged?.Invoke(value);
        }
    }

    public float Speed => m_speed;
    public float MovementDirection => m_movementDirection;

    public event Action<float> OnSliderValueChanged;
    public event Action<bool> OnPointerDown;

    [SerializeField] private UISlider m_uiSlider;
    [SerializeField] private PhysicalSlider m_physicalSlider;

    [SerializeField] private OneEuroFilterSettings m_speedFilterSettings;

    [SerializeField, Disable] private float m_value;
    [SerializeField, Disable] private float m_speed;
    [SerializeField, Disable] private float m_movementDirection;

    [SerializeField, Disable] private int m_pointerDownCount;

    [NonSerialized] private float m_previousValue;
    [NonSerialized] private OneEuroFilter m_speedFilter;

    private void Awake()
    {
        m_uiSlider.OnValueChanged += OnUISliderValueChanged;
        m_uiSlider.OnPointerDown += OnSliderPointerDown;

        m_physicalSlider.OnValueChanged += OnPhysicalSliderValueChanged;
        m_physicalSlider.OnPointerDownChanged += OnSliderPointerDown;

        m_speedFilter = new OneEuroFilter(m_speedFilterSettings.MinCutoff, m_speedFilterSettings.Beta);
        m_speed = m_speedFilter.Filter(0.0f, 0.0f);

        m_pointerDownCount = 0;

    }

    private void OnDestroy()
    {
        m_uiSlider.OnValueChanged -= OnUISliderValueChanged;
        m_uiSlider.OnPointerDown -= OnSliderPointerDown;

        m_physicalSlider.OnValueChanged -= OnPhysicalSliderValueChanged;
        m_physicalSlider.OnPointerDownChanged -= OnSliderPointerDown;
    }

    private void Update()
    {
        UpdateSliderSpeed(Time.deltaTime);
    }

    private void OnPhysicalSliderValueChanged(float newValue)
    {
        m_value = newValue;
        m_uiSlider.SetSliderValue(newValue);
        OnSliderValueChanged?.Invoke(newValue);
    }

    private void OnUISliderValueChanged(float newValue)
    {
        m_value = newValue;
        m_physicalSlider.SetSliderValue(newValue);
        OnSliderValueChanged?.Invoke(newValue);
    }

    private void OnSliderPointerDown(bool fingerDown)
    {
        m_pointerDownCount += fingerDown ? 1 : -1;


        switch (m_pointerDownCount, fingerDown)
        {
            case (0, _):
                OnPointerDown?.Invoke(false);
                break;

            case (1, true):
                OnPointerDown?.Invoke(true);
                break;

            default:
                break;
        }
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
