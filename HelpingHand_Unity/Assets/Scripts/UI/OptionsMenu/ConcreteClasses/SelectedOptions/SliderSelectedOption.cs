using System.Globalization;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;

public class SliderSelectedOption : BaseSelectedOption<float>
{
    [Title("Components")]
    [SerializeField] private Slider m_slider;
    [SerializeField] private Button m_leftButton;
    [SerializeField] private Button m_rightButton;

    [Title("Parameters")]
    [SerializeField] private float m_minValue = 0;
    [SerializeField] private float m_maxValue = 1;
    [SerializeField] private float m_increment = 0.1f;
    [SerializeField] private bool m_wholeNumbers = false;
    [SerializeField] private int m_intIncrement = 1;

    public override void Init(BaseOptionController<float> controller, float startValue)
    {
        base.Init(controller, startValue);

        m_slider.minValue = m_minValue;
        m_slider.maxValue = m_maxValue;
        m_slider.wholeNumbers = m_wholeNumbers;

        m_slider.onValueChanged.AddListener(OnSliderValueChanged);
        m_leftButton.onClick.AddListener(OnMoveLeft);
        m_rightButton.onClick.AddListener(OnMoveRight);

        m_slider.value = startValue;
    }

    public override void Dispose()
    {
        base.Dispose();

        m_slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        m_leftButton.onClick.RemoveListener(OnMoveLeft);
        m_rightButton.onClick.RemoveListener(OnMoveRight);
    }

    protected override void OnMoveRight()
    {
        base.OnMoveRight();
        m_slider.value += m_wholeNumbers ? m_intIncrement : m_increment;
    }

    protected override void OnMoveLeft()
    {
        base.OnMoveLeft();
        m_slider.value -= m_wholeNumbers ? m_intIncrement : m_increment;
    }

    private void OnSliderValueChanged(float newValue)
    {
        SetValue(newValue);
        SelectIfNeeded();
    }

    protected override string ValueToDisplayString(float value)
    {
        return value.ToString(m_wholeNumbers ? "N0" : "N1", CultureInfo.InvariantCulture);
    }
}
