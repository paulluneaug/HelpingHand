using System;
using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;

public abstract class UIOptionEnumController<TEnum> :
    UIAbstractOption<TEnum> where TEnum : struct
{
    [Title("Components")]
    [SerializeField] private TMP_Text m_selectedChoiceText;

    [SerializeField] private Button m_defaultButton;
    [SerializeField] private Button m_leftButton;
    [SerializeField] private Button m_rightButton;

    [Title("Preferences")]
    [SerializeField] private string m_preferenceName;
    [SerializeField] private TEnum m_defaultValue;

    [NonSerialized] private TEnum[] m_choiceList;

    [NonSerialized] private int m_currentSelectedIndex;

    private IEnumerator Start()
    {
        m_defaultButton.onClick.AddListener(SetDefault);
        m_leftButton.onClick.AddListener(OnLeft);
        m_rightButton.onClick.AddListener(OnRight);

        m_choiceList = (TEnum[])Enum.GetValues(typeof(TEnum));

        // This needs to be after GameManager registers to the "game speed" observable float and I don't have to to make it clean
        yield return null;
        var value = PlayerPrefs.GetString(m_preferenceName, m_defaultValue.ToString());
        m_currentSelectedIndex = ValueToIndex(Enum.Parse<TEnum>(value));
        OnIndexChanged();
    }

    public override void SetDefault()
    {
        m_currentSelectedIndex = ValueToIndex(m_defaultValue);
        OnIndexChanged();
    }

    private void OnRight()
    {
        m_currentSelectedIndex = Mod(m_currentSelectedIndex + 1, m_choiceList.Length);
        OnIndexChanged();
    }

    private void OnLeft()
    {
        m_currentSelectedIndex = Mod(m_currentSelectedIndex - 1, m_choiceList.Length);
        OnIndexChanged();
    }

    private void OnIndexChanged()
    {
        var value = IndexToValue(m_currentSelectedIndex);
        m_selectedChoiceText.text = value.ToString();
        PlayerPrefs.SetString(m_preferenceName, value.ToString());
        TriggerValueChanged(value);
    }

    private TEnum IndexToValue(int index)
    {
        return m_choiceList[index];
    }

    private int ValueToIndex(TEnum value)
    {
        return m_choiceList.IndexOf(value);
    }

    private int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}
