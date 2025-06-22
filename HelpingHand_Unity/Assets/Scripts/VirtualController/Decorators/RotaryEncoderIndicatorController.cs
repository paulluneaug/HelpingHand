using System;

using UnityEngine;

using UnityUtility.Extensions;
using UnityUtility.MathU;

public class RotaryEncoderIndicatorController : MonoBehaviour
{
    [SerializeField] private VirtualRotaryEncoder m_controllingEncoder;
    [SerializeField] private VirtualIndicator[] m_indicators;

    [SerializeField] private int m_startSelectedIndicator = 0;

    [NonSerialized] private int m_selectedIndicator = 0;

    private void Start()
    {
        m_selectedIndicator = m_startSelectedIndicator;
        m_controllingEncoder.OnValueChanged += OnEncoderValueChanged;
        OnEncoderValueChanged(0);
    }

    private void OnDestroy()
    {
        m_controllingEncoder.OnValueChanged -= OnEncoderValueChanged;
    }

    private void OnEncoderValueChanged(int offset)
    {
        m_selectedIndicator = (m_selectedIndicator + MathUf.Sign(offset) + m_indicators.Length) % m_indicators.Length;

        m_indicators.ForEach(indicator => indicator.SetEnable(false));
        m_indicators[m_selectedIndicator].SetEnable(true);
    }
}
