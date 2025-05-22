using UnityEngine;

using UnityUtility.Singletons;

public class CanvasManager : MonoBehaviourSingleton<CanvasManager>
{
    [SerializeField] private MainMenuManager m_mainMenuManager;
    [SerializeField] private OptionsMenuController m_optionsMenuController;

    protected override void Awake()
    {
        base.Awake();
        m_mainMenuManager.gameObject.SetActive(true);
        m_optionsMenuController.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        m_mainMenuManager.gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void OpenOptions()
    {
        m_optionsMenuController.gameObject.SetActive(true);
    }

    public void CloseOptions()
    {
        m_optionsMenuController.gameObject.SetActive(false);
    }
}
