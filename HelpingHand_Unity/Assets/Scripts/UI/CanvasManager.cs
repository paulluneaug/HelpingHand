using System;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.CustomAttributes;

public class CanvasManager : MonoBehaviour
{
    [Title("Panels")]
    [SerializeField] private MainMenuManager m_mainMenuController;
    [SerializeField] private OptionsMenuController m_optionsMenuController;

    [Title("Actions")]
    [SerializeField] private InputActionReference m_pauseAction;



    public void Initialize()
    {
        m_optionsMenuController.CloseOptionMenu();
        m_mainMenuController.CloseMainMenu();


        m_pauseAction.action.performed += OnOptionActionPerformed;
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
        m_pauseAction.action.performed -= OnOptionActionPerformed;
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

    private void OnOptionActionPerformed(InputAction.CallbackContext context)
    {
        OpenOptions();
    }
}
