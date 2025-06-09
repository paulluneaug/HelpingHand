using UnityEngine;

using UnityUtility.Singletons;

public class CanvasManager : MonoBehaviourSingleton<CanvasManager>
{
    [SerializeField] private MainMenuManager m_mainMenuController;
    [SerializeField] private OptionsMenuController m_optionsMenuController;

    protected override void Awake()
    {
        base.Awake();
        m_mainMenuController.gameObject.SetActive(true);
        m_optionsMenuController.CloseOptionMenu();
    }

    public void StartGame()
    {
        m_mainMenuController.gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void OpenOptions()
    {
        m_optionsMenuController.OpenOptionMenu();
    }

    public void CloseOptions()
    {
        m_optionsMenuController.CloseOptionMenu();
    }
}
