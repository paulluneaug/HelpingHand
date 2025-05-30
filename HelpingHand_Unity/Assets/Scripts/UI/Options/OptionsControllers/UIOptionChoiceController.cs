using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;

public class UIOptionChoiceController : UIAbstractOption<string>
{
    [Title("Components")]
    [SerializeField] private TMP_Text m_selectedChoiceText;

    [SerializeField] private Button m_defaultButton;
    [SerializeField] private Button m_leftButton;
    [SerializeField] private Button m_rightButton;

    [Title("Parameters")]
    [SerializeField] private string[] m_choiceList;

    [Title("Preferences")]
    [SerializeField] private string m_preferenceName;

    [SerializeField] private string m_defaultValue;

    private int m_currentSelectedIndex;

    private IEnumerator Start()
    {

        m_defaultButton.onClick.AddListener(SetDefault);
        m_leftButton.onClick.AddListener(OnLeft);
        m_rightButton.onClick.AddListener(OnRight);

        // This needs to be after GameManager registers to the "game speed" observable float and I don't have to to make it clean
        yield return null;
        var value = PlayerPrefs.GetString(m_preferenceName, m_defaultValue);
        m_currentSelectedIndex = ValueToIndex(value);
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
        m_selectedChoiceText.text = value;
        PlayerPrefs.SetString(m_preferenceName, value);
        TriggerValueChanged(value);
    }

    private string IndexToValue(int index)
    {
        return m_choiceList[index];
    }

    private int ValueToIndex(string value)
    {
        return m_choiceList.IndexOf(value);
    }

    private int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

}