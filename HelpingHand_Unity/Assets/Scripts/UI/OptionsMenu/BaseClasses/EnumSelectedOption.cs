using System;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;

public class EnumSelectedOption<TEnum> : BaseSelectedOption<TEnum>
    where TEnum : struct
{
    [Title("Components")]
    [SerializeField] private Button m_leftButton;
    [SerializeField] private Button m_rightButton;

    [NonSerialized] private TEnum[] m_choiceList;

    [NonSerialized] private int m_currentSelectedIndex;

    public override void Init(BaseOptionController<TEnum> controller, TEnum startValue)
    {
        base.Init(controller, startValue);

        m_leftButton.onClick.AddListener(OnMoveLeft);
        m_rightButton.onClick.AddListener(OnMoveRight);

        m_choiceList = (TEnum[])Enum.GetValues(typeof(TEnum));

        m_currentSelectedIndex = ValueToIndex(startValue);
        OnIndexChanged();
    }

    protected override void OnMoveRight()
    {
        base.OnMoveRight();
        m_currentSelectedIndex = Mod(m_currentSelectedIndex + 1, m_choiceList.Length);
        OnIndexChanged();
        SelectIfNeeded();
    }

    protected override void OnMoveLeft()
    {
        base.OnMoveLeft();
        m_currentSelectedIndex = Mod(m_currentSelectedIndex - 1, m_choiceList.Length);
        OnIndexChanged();
        SelectIfNeeded();
    }

    private void OnIndexChanged()
    {
        TEnum value = IndexToValue(m_currentSelectedIndex);
        SetValue(value);
    }

    private TEnum IndexToValue(int index)
    {
        return m_choiceList[index];
    }

    private int ValueToIndex(TEnum value)
    {
        return m_choiceList.IndexOf(value);
    }

    private static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}
