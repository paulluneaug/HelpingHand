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
    [SerializeField] private InputActionReference m_pauseAction;



    public void Initialize()
    {
        m_optionsMenuController.CloseOptionMenu();
        m_mainMenuController.CloseMainMenu();
        m_creditsMenuController.CloseCreditsMenu();


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

    public void CloseCredits()
    {
        m_mainMenuController.OpenMainMenu();
        m_creditsMenuController.CloseCreditsMenu();
    }

    public void OpenCredits()
    {
        m_creditsMenuController.OpenCreditsMenu();
        m_mainMenuController.CloseMainMenu();
    }
}
