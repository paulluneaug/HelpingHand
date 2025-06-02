using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UIOptionToggleController : UIAbstractOption<bool>
{
    [Header("Components")]
    [SerializeField] private Toggle _toggle;

    [SerializeField] private Button _defaultButton;
    [SerializeField] private TMP_Text _valueText;

    [Header("Preferences")]
    [SerializeField] private string _preferenceName;
    [SerializeField] private bool _defaultValue;


    private IEnumerator Start()
    {
        _toggle.onValueChanged.AddListener(OnValueChanged);
        _defaultButton.onClick.AddListener(SetDefault);

        // This needs to be after GameManager registers to the "game speed" observable float and I don't have to to make it clean
        yield return null;
        var value = IntToBool(PlayerPrefs.GetInt(_preferenceName, BoolToInt(_defaultValue)));
        _toggle.isOn = value;
    }

    private void OnValueChanged(bool value)
    {
        PlayerPrefs.SetInt(_preferenceName, BoolToInt(value));
        _valueText.text = value ? "On" : "Off";
        TriggerValueChanged(value);
    }

    private bool IntToBool(int value)
    {
        return value != 0;
    }

    private int BoolToInt(bool value)
    {
        return value ? 1 : 0;
    }

    public override void SetDefault()
    {
        _toggle.isOn = _defaultValue;
    }
}