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
        Action mainMenuAction = GameManager.Instance.CurrentGameState switch
        {
            GameManager.GameState.MainMenu => m_mainMenuController.OpenMainMenu,
            GameManager.GameState.Gameplay => m_mainMenuController.CloseMainMenu,
            _ => throw new NotImplementedException(),
        };
        mainMenuAction();

        m_optionsMenuController.CloseOptionMenu();

        m_pauseAction.action.performed += OnOptionActionPerformed;
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
        m_optionsMenuController.OpenOptionMenu();
    }

    public void CloseOptions()
    {
        m_optionsMenuController.CloseOptionMenu();
    }

    private void OnOptionActionPerformed(InputAction.CallbackContext context)
    {
        OpenOptions();
    }
}
