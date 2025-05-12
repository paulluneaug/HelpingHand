using System;


using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button m_startGameButton;
    [SerializeField] private Button m_openOptionsButton;
    [SerializeField] private Button m_quitGameButton;


    private void OnEnable()
    {
        m_startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        m_openOptionsButton.onClick.AddListener(OnOpenOptionsButtonClicked);
        m_quitGameButton.onClick.AddListener(OnQuitGameButtonClicked);
    }

    private void OnDisable()
    {
        m_startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
        m_openOptionsButton.onClick.RemoveListener(OnOpenOptionsButtonClicked);
        m_quitGameButton.onClick.RemoveListener(OnQuitGameButtonClicked);
    }

    private void OnStartGameButtonClicked()
    {
        CanvasManager.Instance.StartGame();
    }

    private void OnOpenOptionsButtonClicked()
    {
        CanvasManager.Instance.OpenOptions();
    }

    private void OnQuitGameButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }
}
