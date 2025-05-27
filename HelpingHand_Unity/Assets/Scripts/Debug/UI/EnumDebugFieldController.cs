using System;
using System.Linq;

using TMPro;

using UnityEngine;

public class EnumDebugFieldController<TEnum> : MonoBehaviour
    where TEnum : struct
{
    [SerializeField] private TMP_Text m_label;
    [SerializeField] private TMP_Dropdown m_dropdown;

    public void Init(string labelName)
    {
        m_label.text = labelName;
        m_dropdown.options = Enum.GetNames(typeof(TEnum)).Select(enumValue => new TMP_Dropdown.OptionData(enumValue)).ToList();
    }

    public void Init(string labelName, TEnum selectedOption)
    {
        Init(labelName);
        m_dropdown.value = m_dropdown.options.FindIndex(option => option.text == selectedOption.ToString());

    }

    public TEnum GetValue()
    {
        TMP_Dropdown.OptionData selectedOption = m_dropdown.options[m_dropdown.value];
        return Enum.Parse<TEnum>(selectedOption.text);
    }
}
