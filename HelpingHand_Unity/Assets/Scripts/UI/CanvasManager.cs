using System;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.CustomAttributes;

public class CanvasManager : MonoBehaviour
{
    [Title("Panels")]
    [SerializeField] private MainMenuController m_mainMenuController;
    [SerializeField] private OptionsMenuController m_optionsMenuController;
    [SerializeField] private CreditsMenuController m_creditsMenuController;

    [Title("Actions")]
    [SerializeField] private ButtonInputEvent m_pauseEvent;

    [NonSerialized] private bool m_paused;



    public void Initialize()
    {
        m_optionsMenuController.CloseOptionMenu();
        m_mainMenuController.CloseMainMenu();
        m_creditsMenuController.CloseCreditsMenu();


        m_pauseEvent.AddDownListener(OnPause);
        m_paused = false;
    }

    private void Start()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameManager.GameState.MainMenu:
                m_mainMenuController.OpenMainMenu();
                break;
            case GameManager.GameState.Gameplay:
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        m_pauseEvent.RemoveDownListener(OnPause);
    }

    public void StartGame()
    {
        m_mainMenuController.CloseMainMenu();
        GameManager.Instance.StartGameplay();
    }

    public void OpenOptions()
    {
        m_mainMenuController.CloseMainMenu();
        m_optionsMenuController.OpenOptionMenu();
    }

    public void CloseOptions()
    {
        m_optionsMenuController.CloseOptionMenu();
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.MainMenu)
        {
            m_mainMenuController.OpenMainMenu();
        }
    }

    private void OnPause()
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager.CurrentGameState != GameManager.GameState.Gameplay)
        {
            return;
        }

        m_paused = !m_paused;
        gameManager.Paused.Value = m_paused;
        if (m_paused)
        {
            OpenOptions();
        }
        else
        {
            CloseOptions();
        }
    }

    public void CloseCredits()
    {
        m_creditsMenuController.CloseCreditsMenu();
        m_mainMenuController.OpenMainMenu();
    }

    public void OpenCredits()
    {
        m_mainMenuController.CloseMainMenu();
        m_creditsMenuController.OpenCreditsMenu();
    }
}
