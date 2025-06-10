using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_mainMenuPanel;

    [SerializeField] private Button m_startGameButton;
    [SerializeField] private Button m_openOptionsButton;
    [SerializeField] private Button m_quitGameButton;


    public void OpenMainMenu()
    {
        m_mainMenuPanel.alpha = 1;
        m_mainMenuPanel.interactable = true;
        m_mainMenuPanel.blocksRaycasts = true;

        m_startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        m_openOptionsButton.onClick.AddListener(OnOpenOptionsButtonClicked);
        m_quitGameButton.onClick.AddListener(OnQuitGameButtonClicked);
    }

    public void CloseMainMenu()
    {
        m_mainMenuPanel.alpha = 0;
        m_mainMenuPanel.interactable = false;
        m_mainMenuPanel.blocksRaycasts = false;

        m_startGameButton.onClick.RemoveListener(OnStartGameButtonClicked);
        m_openOptionsButton.onClick.RemoveListener(OnOpenOptionsButtonClicked);
        m_quitGameButton.onClick.RemoveListener(OnQuitGameButtonClicked);
    }

    private void OnStartGameButtonClicked()
    {
        GameManager.Instance.CanvasManager.StartGame();
    }

    private void OnOpenOptionsButtonClicked()
    {
        GameManager.Instance.CanvasManager.OpenOptions();
    }

    private void OnQuitGameButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }
}
