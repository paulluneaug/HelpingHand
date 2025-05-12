using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.Extensions;

public class UIOptionChoiceController : UIAbstractOption<string>
{
    [Header("Components")]
    [SerializeField] private TMP_Text _selectedChoiceText;

    [SerializeField] private Button _defaultButton;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [Header("Parameters")]
    [SerializeField] private string[] _choiceList;

    [Header("Preferences")]
    [SerializeField] private string _preferenceName;

    [SerializeField] private string _defaultValue;

    private int m_currentSelectedIndex;

    private IEnumerator Start()
    {

        _defaultButton.onClick.AddListener(SetDefault);
        _leftButton.onClick.AddListener(OnLeft);
        _rightButton.onClick.AddListener(OnRight);

        // This needs to be after GameManager registers to the "game speed" observable float and I don't have to to make it clean
        yield return null;
        var value = PlayerPrefs.GetString(_preferenceName, _defaultValue);
        m_currentSelectedIndex = ValueToIndex(value);
        OnIndexChanged();
    }

    public override void SetDefault()
    {
        m_currentSelectedIndex = ValueToIndex(_defaultValue);
        OnIndexChanged();
    }

    private void OnRight()
    {
        m_currentSelectedIndex = Mod(m_currentSelectedIndex + 1, _choiceList.Length);
        OnIndexChanged();
    }

    private void OnLeft()
    {
        m_currentSelectedIndex = Mod(m_currentSelectedIndex - 1, _choiceList.Length);
        OnIndexChanged();
    }

    private void OnIndexChanged()
    {
        var value = IndexToValue(m_currentSelectedIndex);
        _selectedChoiceText.text = value;
        PlayerPrefs.SetString(_preferenceName, value);
        TriggerValueChanged(value);
    }

    private string IndexToValue(int index)
    {
        return _choiceList[index];
    }

    private int ValueToIndex(string value)
    {
        return _choiceList.IndexOf(value);
    }

    private int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

}