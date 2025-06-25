using UnityEngine;

using UnityUtility.CustomAttributes;

public class IndicatorsController : MonoBehaviour
{
    [SerializeField] private BaseVariable<bool>[] m_indicators;

    [SerializeField, Disable] private int m_indicatorState;

    private void Update()
    {
        int currentState = GetIndicatorsState();
        if (m_indicatorState != currentState)
        {
            m_indicatorState = currentState;
            GameManager.Instance.ArduinoConnectorManager.SendIndicatorState(currentState);
        }
    }

    private int GetIndicatorsState()
    {
        int indicatorState = 0;

        for (int iIndicator = 0; iIndicator < m_indicators.Length; iIndicator++)
        {
            indicatorState |= (!m_indicators[iIndicator].Value) ? 0 : (1 << iIndicator);
        }

        return indicatorState;
    }
}
