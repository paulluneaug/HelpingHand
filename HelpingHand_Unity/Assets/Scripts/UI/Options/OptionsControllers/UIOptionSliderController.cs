using System.Collections;
using System.Globalization;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UIOptionSliderController : UIAbstractOption<float>
{
    [Header("Components")]
    [SerializeField] private Slider m_slider;
    [SerializeField] private Button m_defaultButton;
    [SerializeField] private Button m_leftButton;
    [SerializeField] private Button m_rightButton;
    [SerializeField] private TMP_Text m_valueText;

    [Header("Parameters")]
    [SerializeField] private float m_minValue = 0;
    [SerializeField] private float m_maxValue = 1;
    [SerializeField] private float m_increment = 0.1f;
    [SerializeField] private bool m_wholeNumbers = false;
    [SerializeField] private float m_intIncrement = 1f;

    [Header("Preferences")]
    [SerializeField] private string m_preferenceName;
    [SerializeField] private float m_defaultValue;

    private IEnumerator Start()
    {
        m_slider.minValue = m_minValue;
        m_slider.maxValue = m_maxValue;
        m_slider.wholeNumbers = m_wholeNumbers;
        m_slider.onValueChanged.AddListener(OnValueChanged);
        m_defaultButton.onClick.AddListener(SetDefault);
        m_leftButton.onClick.AddListener(OnLeft);
        m_rightButton.onClick.AddListener(OnRight);

        // This needs to be after GameManager registers to the "game speed" observable float and I don't have to to make it clean
        yield return null;
        var value = PlayerPrefs.GetFloat(m_preferenceName, m_defaultValue);
        m_slider.value = value;
    }

    private void OnValueChanged(float value)
    {
        PlayerPrefs.SetFloat(m_preferenceName, value);
        m_valueText.text = value.ToString(m_wholeNumbers ? "N0" : "N1", CultureInfo.InvariantCulture);
        TriggerValueChanged(value);
    }

    private void OnRight()
    {
        m_slider.value += m_wholeNumbers ? m_intIncrement : m_increment;
    }

    private void OnLeft()
    {
        m_slider.value -= m_wholeNumbers ? m_intIncrement : m_increment;
    }

    public override void SetDefault()
    {
        m_slider.value = m_defaultValue;
    }
}
