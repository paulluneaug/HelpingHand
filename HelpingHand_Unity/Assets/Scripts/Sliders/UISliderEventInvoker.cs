using UnityEngine;

[RequireComponent(typeof(UISlider))]
public class UISliderEventInvoker : MonoBehaviour
{
    [SerializeField]
    private FloatVariable m_floatVariable;

    private UISlider m_uiSlider;

    private void Awake()
    {
        m_uiSlider = GetComponent<UISlider>();
    }

    private void Start()
    {
        m_floatVariable.SetValueWithoutNotify(m_uiSlider.Value);
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
        m_floatVariable.Value = value;
    }
}
