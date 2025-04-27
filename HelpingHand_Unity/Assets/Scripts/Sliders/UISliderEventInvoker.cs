using Events;

using UnityEngine;

[RequireComponent(typeof(UISlider))]
public class UISliderEventInvoker : MonoBehaviour
{
    [SerializeField]
    private FloatGameEvent m_event;
    
    private UISlider m_uiSlider;
    
    private void Awake()
    {
        m_uiSlider = GetComponent<UISlider>();
    }

    private void OnEnable()
    {
        m_uiSlider.OnValueChanged += OnSliderValueChanged;
    }

    private void OnDisable()
    {
        m_uiSlider.OnValueChanged -= OnSliderValueChanged;
    }

    private void OnSliderValueChanged(float value)
    {
        m_event.Raise(value);
    }
}
