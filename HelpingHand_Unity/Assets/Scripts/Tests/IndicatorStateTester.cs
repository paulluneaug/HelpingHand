using UnityEngine;

using UnityUtility.CustomAttributes;

public class IndicatorStateTester : MonoBehaviour
{
    [SerializeField] private ArduinoConnectorManager m_connectorManager;

    [SerializeField] private bool[] m_indicators;

    [SerializeField, Disable] private int m_indicatorState;

    private void Update()
    {
        int currentState = GetIndicatorsState();
        if (m_indicatorState != currentState)
        {
            m_indicatorState = currentState;
            m_connectorManager.SendIndicatorState(currentState);
        }
    }

    private int GetIndicatorsState()
    {
        int indicatorState = 0;

        for (int iIndicator = 0; iIndicator < m_indicators.Length; iIndicator++)
        {
            indicatorState |= (!m_indicators[iIndicator]) ? 0 : (1 << iIndicator);
        }

        return indicatorState;
    }
}
