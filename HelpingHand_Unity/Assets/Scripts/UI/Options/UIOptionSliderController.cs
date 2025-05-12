using System.Collections;
using System.Globalization;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UIOptionSliderController : UIAbstractOption<float>
{
    [Header("Components")]
    [SerializeField] private Slider _slider;
    [SerializeField] private Button _defaultButton;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private TMP_Text _valueText;

    [Header("Parameters")]
    [SerializeField] private float _minValue = 0;
    [SerializeField] private float _maxValue = 1;
    [SerializeField] private float _increment = 0.1f;
    [SerializeField] private bool _wholeNumbers = false;
    [SerializeField] private float _intIncrement = 1f;

    [Header("Preferences")]
    [SerializeField] private string _preferenceName;
    [SerializeField] private float _defaultValue;

    private IEnumerator Start()
    {
        _slider.minValue = _minValue;
        _slider.maxValue = _maxValue;
        _slider.wholeNumbers = _wholeNumbers;
        _slider.onValueChanged.AddListener(OnValueChanged);
        _defaultButton.onClick.AddListener(SetDefault);
        _leftButton.onClick.AddListener(OnLeft);
        _rightButton.onClick.AddListener(OnRight);

        // This needs to be after GameManager registers to the "game speed" observable float and I don't have to to make it clean
        yield return null;
        var value = PlayerPrefs.GetFloat(_preferenceName, _defaultValue);
        _slider.value = value;
    }

    private void OnValueChanged(float value)
    {
        PlayerPrefs.SetFloat(_preferenceName, value);
        _valueText.text = value.ToString(_wholeNumbers ? "N0" : "N1", CultureInfo.InvariantCulture);
        TriggerValueChanged(value);
    }

    private void OnRight()
    {
        _slider.value += _wholeNumbers ? _intIncrement : _increment;
    }

    private void OnLeft()
    {
        _slider.value -= _wholeNumbers ? _intIncrement : _increment;
    }

    public override void SetDefault()
    {
        _slider.value = _defaultValue;
    }
}
