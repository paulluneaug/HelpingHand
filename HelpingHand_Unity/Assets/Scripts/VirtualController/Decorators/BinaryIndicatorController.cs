using System;

using UnityEngine;

using UnityUtility.Extensions;

public class BinaryIndicatorController : MonoBehaviour
{
    [SerializeField] private VirtualInput<bool> m_controllingInput0;
    [SerializeField] private VirtualInput<bool> m_controllingInput1;

    [SerializeField] private VirtualIndicator m_indicator0;
    [SerializeField] private VirtualIndicator m_indicator1;
    [SerializeField] private VirtualIndicator m_indicator2;
    [SerializeField] private VirtualIndicator m_indicator3;

    [NonSerialized] private VirtualIndicator[] m_indicators;

    private void Awake()
    {
        m_indicators = new VirtualIndicator[]
        {
            m_indicator0,
            m_indicator1,
            m_indicator2,
            m_indicator3,
        };

        m_controllingInput0.OnValueChanged += UpdateIndicators;
        m_controllingInput1.OnValueChanged += UpdateIndicators;

        UpdateIndicators(false);
    }

    private void OnDestroy()
    {
        m_controllingInput0.OnValueChanged -= UpdateIndicators;
        m_controllingInput1.OnValueChanged -= UpdateIndicators;
    }

    private void UpdateIndicators(bool _)
    {
        int indicatorState = (m_controllingInput0.Value ? 1 << 0 : 0) |
                             (m_controllingInput1.Value ? 1 << 1 : 0);

        m_indicators.ForEach(indicator => indicator.SetEnable(false));
        m_indicators[indicatorState].SetEnable(true);
    }
}
