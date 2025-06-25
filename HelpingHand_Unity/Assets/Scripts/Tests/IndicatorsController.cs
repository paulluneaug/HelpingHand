using System;

using UnityEngine;

using UnityUtility.CustomAttributes;

public class IndicatorsController : MonoBehaviour
{
    [SerializeField] private BaseVariable<bool>[] m_indicators;

    [SerializeField, Disable] private int m_indicatorState;

    [NonSerialized] private bool m_firstSend = false;

    private void Start()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager.CurrentGameState != GameManager.GameState.Gameplay)
        {
            gameManager.OnGameStateChanged += OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.OnGameStateChanged -= OnGameStateChanged;
        m_indicatorState = GetIndicatorsState();
        GameManager.Instance.ArduinoConnectorManager.SendIndicatorState(m_indicatorState);
        m_firstSend = true;
    }

    private void Update()
    {
        if (!m_firstSend)
        {
            return;
        }

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
