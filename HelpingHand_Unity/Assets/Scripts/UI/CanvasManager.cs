using UnityEngine;

using UnityUtility.Singletons;

public class CanvasManager : MonoBehaviourSingleton<CanvasManager>
{
    [SerializeField] private MainMenuManager m_mainMenuManager;
    [SerializeField] private OptionsMenuManager m_optionsMenuManager;

    protected override void Awake()
    {
        base.Awake();
        m_mainMenuManager.gameObject.SetActive(true);
        m_optionsMenuManager.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        m_mainMenuManager.gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void OpenOptions()
    {
        m_optionsMenuManager.gameObject.SetActive(true);
    }

    public void CloseOptions()
    {
        m_optionsMenuManager.gameObject.SetActive(false);
    }
}
