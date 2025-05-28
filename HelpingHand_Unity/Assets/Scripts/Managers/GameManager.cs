using System;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityUtility.SceneReference;
using UnityUtility.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private enum GameState
    {
        Idle,
        MainMenu,
        Gameplay,
    }

    public ActSequenceManager ActSequenceManager => m_actSequenceManager;


    [Title("Sub Managers", titleAlignment: TitleAlignments.Centered)]

    [SerializeField] private ActSequenceManager m_actSequenceManager;

    [Title("Start")]
    [SerializeField] private GameState m_startGameState;

    [Title("Scene References")]
    [SerializeField] private SceneReference m_globalObjectsScene;

    // Cache
    [NonSerialized] private Puppet m_puppet;
    [NonSerialized] private GameState m_currentGameState;


    public override void Initialize()
    {
        base.Initialize();
        m_actSequenceManager.Initialize();
        LoadGlobalObjectScene();

        m_currentGameState = m_startGameState;
    }

    protected override void Start()
    {
        base.Start();
        switch (m_currentGameState)
        {
            case GameState.Idle:
                break;
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
        m_actSequenceManager.StartSequence();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_currentGameState != GameState.Gameplay)
            {
                StartGameplay();
            }
        }
        
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

}
