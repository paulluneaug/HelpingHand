using System;
using System.Collections.Generic;
using System.IO;

using Cysharp.Threading.Tasks;

using Events;

using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityUtility.ObservableFields;
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

    [Serializable]
    private class GeneralSettings
    {
        public bool EnableVirtualController;
    }

    public ActSequenceManager ActSequenceManager => m_actSequenceManager;
    public GameOptionsManager GameOptionsManager => m_gameOptionsManager;
    public CanvasManager CanvasManager => m_canvasManager;
    public ArduinoConnectorManager ArduinoConnectorManager => m_arduinoConnectorManager;

    public ButtonInputEvent SkipDialogueInput => m_skipDialogueInput;

    public GameState CurrentGameState => m_currentGameState;

    public bool VirtualControllerEnabled => m_virtualControllerEnabled;

    public event Action<GameState> OnGameStateChanged;

    [NonSerialized] public ObservableField<bool> Paused;

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
    [SerializeField] private SceneReference m_virtualControllerScene;
    [Separator]

    [Title("Input References")]
    [SerializeField] private ButtonInputEvent m_skipDialogueInput;

    [Title("Misc")]
    [SerializeField] private Transform m_puppetStart;
    [SerializeField] private string m_generalSettingsPath;
    
    [Title("Initialization")]
    [SerializeField]
    private List<BaseGameEvent> m_allGameEvents;

    // Cache
    [NonSerialized] private Puppet m_puppet;
    [NonSerialized] private GameState m_currentGameState;
    [NonSerialized] private bool m_virtualControllerEnabled;

    public override void Initialize()
    {
        base.Initialize();

        Paused = new ObservableField<bool>(false);
        Paused.OnValueChanged += OnPausedChanged;


        m_currentGameState = m_startGameState;

        m_gameOptionsManager.Initialize();
        m_actSequenceManager.Initialize();
        m_arduinoConnectorManager.Initialize();
        m_canvasManager.Initialize();

        LoadGlobalObjectScene();

        m_virtualControllerEnabled = false;
        string generalSettingsFullPath = Path.Combine(".", "ExternalAssets", m_generalSettingsPath);
        if (File.Exists(generalSettingsFullPath))
        {
            string generalSettings = File.ReadAllText(generalSettingsFullPath);
            GeneralSettings settings = JsonUtility.FromJson<GeneralSettings>(generalSettings);
            m_virtualControllerEnabled = settings.EnableVirtualController;
            if (settings.EnableVirtualController)
            {
                LoadVirtualControllerScene(); // TODO disable in production build
            }
        }
    }

    private void OnPausedChanged(bool pause)
    {
        if (pause)
        {
            m_actSequenceManager.PauseSequence();
        }
        else
        {
            m_actSequenceManager.ResumeSequence();
        }
    }

    protected override void Start()
    {
        base.Start();
        StartAsync().Forget();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_arduinoConnectorManager.Dispose();
        m_gameOptionsManager.Dispose();
    }

    private async UniTask StartAsync()
    {
        await UniTask.WaitUntil(() => m_arduinoConnectorManager.IsReady);
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
    
#if UNITY_EDITOR
    [Button("Load all game events")]
    private void LoadGameEvents()
    {
        m_allGameEvents = new List<BaseGameEvent>();
        var assetGUIDS = AssetDatabase.FindAssets("t:BaseGameEvent", new string[] { "Assets/Resources/" });
        foreach (string assetGUID in assetGUIDS)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            foreach (UnityEngine.Object obj in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            {
                if (obj is BaseGameEvent gameEvent)
                {
                    m_allGameEvents.Add(gameEvent);
                }
            }
        }
        AssetDatabase.SaveAssetIfDirty(this);
    }
#endif
    
    public void StartGameplay()
    {
        // Initialize all variables
        // BaseGameEvent[] allEvents = Resources.LoadAll<BaseGameEvent>(string.Empty);
        // foreach (BaseGameEvent gameEvent in allEvents)
        // {
        //     gameEvent.Initialize();
        // }
        
        foreach (BaseGameEvent gameEvent in m_allGameEvents)
        {
            gameEvent.Initialize();
        }

        // TODO Initialize all singletons

        m_arduinoConnectorManager.SendFaderPosition(true);

        m_puppet.transform.SetPositionAndRotation(m_puppetStart.position, m_puppetStart.rotation);

        m_currentGameState = GameState.Gameplay;
        OnGameStateChanged?.Invoke(m_currentGameState);

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
        SceneManager.LoadScene(m_globalObjectsScene, LoadSceneMode.Additive);
#else
        SceneManager.LoadScene(m_globalObjectsScene, LoadSceneMode.Additive);
#endif
    }

    private void LoadVirtualControllerScene()
    {
#if UNITY_EDITOR
        if (SceneManager.GetSceneByPath(m_virtualControllerScene.ScenePath) != default)
        {
            return;
        }
        SceneManager.LoadScene(m_virtualControllerScene, LoadSceneMode.Additive);
#else
        SceneManager.LoadScene(m_virtualControllerScene, LoadSceneMode.Additive);
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


    public void ReturnToMainMenu()
    {
        switch (m_currentGameState)
        {
            case GameState.MainMenu:
                m_canvasManager.CloseOptions();
                break;
            case GameState.Gameplay:
                m_currentGameState = GameState.MainMenu;
                m_canvasManager.CloseOptions();
                m_actSequenceManager.StopSequence();
                OnGameStateChanged?.Invoke(m_currentGameState);
                DialogueManager.Instance.CloseDialoguePanel();
                break;
            default:
                break;
        }
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
