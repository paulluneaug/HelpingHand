using System;

using DG.Tweening.Core.Easing;

using UnityEngine;

public class VariableWriter<T> : MonoBehaviour
{
    [SerializeField] private BaseVariable<T> m_variable;
    [SerializeField] private T m_value;

    private void Start()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager.CurrentGameState != GameManager.GameState.Gameplay)
        {
            gameManager.OnGameStateChanged += OnGameStateChanged;
            return;
        }
        m_variable.Value = m_value;
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        if (state != GameManager.GameState.Gameplay)
        {
            return;
        }

        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        m_variable.Value = m_value;
    }
}
