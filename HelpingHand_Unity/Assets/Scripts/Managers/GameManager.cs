using System;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using UnityUtility.SceneReference;
using UnityUtility.Singletons;

using Separator = UnityUtility.CustomAttributes.SeparatorAttribute;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private enum GameState
    {
        MainMenu,
        Gameplay,
    }

    public ActSequenceManager ActSequenceManager => m_actSequenceManager;
    public GameOptionsManager GameOptionsManager => m_gameOptionsManager;

    public InputAction SkipDialogueInput => m_skipDialogueInput.action;


    [Title("Sub Managers", titleAlignment: TitleAlignments.Centered)]

    [SerializeField] private ActSequenceManager m_actSequenceManager;
    [SerializeField] private GameOptionsManager m_gameOptionsManager;

    [Title("Start")]
    [SerializeField] private GameState m_startGameState;

    [Title("Scene References")]
    [SerializeField] private SceneReference m_globalObjectsScene;
    [Separator]

    [Title("Input References")]
    [SerializeField] private InputActionReference m_skipDialogueInput;

    // Cache
    [NonSerialized] private Puppet m_puppet;
    [NonSerialized] private GameState m_currentGameState;

    public override void Initialize()
    {
        base.Initialize();
        m_actSequenceManager.Initialize();
        LoadGlobalObjectScene();

        m_currentGameState = m_startGameState;

        m_skipDialogueInput.asset.Enable();
    }

    protected override void Start()
    {
        base.Start();
        switch (m_currentGameState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Gameplay:
                StartGameplay();
                break;
            default:
                break;
        }
    }

    public void StartGameplay()
    {
        m_currentGameState = GameState.Gameplay;
        m_actSequenceManager.Start();
    }

    private void Update()
    {
        switch (m_currentGameState)
        {
            case GameState.MainMenu:
                UpdateMainMenu();
                break;
            case GameState.Gameplay:
                UpdateGameplay();
                break;
            default:
                break;
        }
    }

    #region Load
    private void LoadGlobalObjectScene()
    {
#if UNITY_EDITOR
        if (SceneManager.GetSceneByPath(m_globalObjectsScene.ScenePath) != default)
        {
            return;
        }
        SceneManager.LoadScene(m_globalObjectsScene);
#else
        SceneManager.LoadScene(m_globalObjectsScene);
#endif
    }

    #endregion

    #region Updates
    private void UpdateMainMenu()
    {

    }

    private void UpdateGameplay()
    {
        m_actSequenceManager.Update(Time.deltaTime);
    }
    #endregion

    #region Puppet
    public Puppet GetPuppet()
    {
        if (m_puppet == null)
        {
            Debug.LogError($"No puppet registered : Call {nameof(RegisterPuppet)}");
            return null;
        }
        return m_puppet;
    }

    public void RegisterPuppet(Puppet puppet)
    {
        if (m_puppet != null)
        {
            Debug.LogError("A puppet was already registered");
            return;
        }
        m_puppet = puppet;
    }

    public void UnregisterPuppet()
    {
        m_puppet = null;
    }

    #endregion

    public void StartGame()
    {
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
