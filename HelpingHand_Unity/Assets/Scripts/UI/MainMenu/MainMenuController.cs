using System;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;

[RequireComponent(typeof(RectTransform))]
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_mainMenuPanel;
    [SerializeField] private Selectable m_firstSelectable;

    [Title("Buttons")]
    [SerializeField] private Button m_startGameButton;
    [SerializeField] private Button m_openOptionsButton;
    [SerializeField] private Button m_creditsOptionsButton;
    [SerializeField] private Button m_quitGameButton;

    [NonSerialized] private bool m_open = false;


    public void OpenMainMenu()
    {
        if (m_open)
        {
            return;
        }

        m_mainMenuPanel.alpha = 1;
        m_mainMenuPanel.interactable = true;
        m_mainMenuPanel.blocksRaycasts = true;

        m_startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        m_openOptionsButton.onClick.AddListener(OnOpenOptionsButtonClicked);
        m_creditsOptionsButton.onClick.AddListener(OnCreditsButtonClicked);
        m_quitGameButton.onClick.AddListener(OnQuitGameButtonClicked);

        m_firstSelectable.Select();

        m_open = true;

    }

    public void CloseMainMenu()
    {
        if (!m_open)
        {
            return;
        }

        m_mainMenuPanel.alpha = 0;
        m_mainMenuPanel.interactable = false;
        m_mainMenuPanel.blocksRaycasts = false;

        m_startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
        m_openOptionsButton.onClick.RemoveListener(OnOpenOptionsButtonClicked);
        m_creditsOptionsButton.onClick.RemoveListener(OnCreditsButtonClicked);
        m_quitGameButton.onClick.RemoveListener(OnQuitGameButtonClicked);

        m_open = false;
    }

    private void OnStartGameButtonClicked()
    {
        GameManager.Instance.CanvasManager.StartGame();
    }

    private void OnOpenOptionsButtonClicked()
    {
        GameManager.Instance.CanvasManager.OpenOptions();
    }

    private void OnCreditsButtonClicked()
    {
        GameManager.Instance.CanvasManager.OpenCredits();
    }

    private void OnQuitGameButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }
}
