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
    public enum GameState
    {
        MainMenu,
        Gameplay,
    }

    public ActSequenceManager ActSequenceManager => m_actSequenceManager;
    public GameOptionsManager GameOptionsManager => m_gameOptionsManager;
    public CanvasManager CanvasManager => m_canvasManager;

    public InputAction SkipDialogueInput => m_skipDialogueInput.action;

    public GameState CurrentGameState => m_currentGameState;

    [Title("Act Sequence Manager")]
    [SerializeField] private ActSequenceManager m_actSequenceManager;

    [Title("Game Options Manager")]
    [SerializeField] private GameOptionsManager m_gameOptionsManager;

    [Title("Arduino Connector Manager")]
    [SerializeField] private ArduinoConnectorManager m_arduinoConnectorManager;

    [Title("Canvas Manager")]
    [SerializeField] private CanvasManager m_canvasManager;

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

        m_currentGameState = m_startGameState;

        m_actSequenceManager.Initialize();
        m_arduinoConnectorManager.Initialize();
        m_canvasManager.Initialize();

        LoadGlobalObjectScene();

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

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_arduinoConnectorManager.Dispose();
    }

    public void StartGameplay()
    {
        m_currentGameState = GameState.Gameplay;

        DialogueManager.Instance.OpenDialoguePanel();

        m_actSequenceManager.StartSequence();
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
        m_actSequenceManager.UpdateSequence(Time.deltaTime);
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


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
