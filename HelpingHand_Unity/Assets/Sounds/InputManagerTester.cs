using UnityEngine;

public class InputManagerTester : MonoBehaviour
{
    public static InputManagerTester Instance;

    private bool m_bIsInitialized = false;

    [Header("Event Input Key Assignments")]
    [SerializeField] private KeyCode StartMainMusic;
    [SerializeField] private KeyCode StopMainMusic;

    [Header("GameState Input Key Assignments")]
    [SerializeField] private KeyCode Game_GameOver;
    [SerializeField] private KeyCode Game_Gameplay;
    [SerializeField] private KeyCode Game_Paused;

    [Header("MusicState Input Key Assignments")]
    [SerializeField] private KeyCode Music_MainMenu;
    [SerializeField] private KeyCode Music_LevelStart;
    [SerializeField] private KeyCode Music_LevelWin;
    [SerializeField] private KeyCode Music_PauseMenu;
    [SerializeField] private KeyCode Music_LevelLose;

    private void Awake()
    {
        Initialize();
    }
    void Initialize()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.LogWarning("De multiples instances de AudioManager ont été trouvées. Veuillez vous assurer qu'il n'y a qu'une seule instance dans la scène.");
            Destroy(this);
        }

        if (!m_bIsInitialized)
        {
            m_bIsInitialized = true;
        }

    }

    private void Update()
    {
        CheckInputs();
    }

    public void CheckInputs()
    {
        if (!Input.anyKey)
            return;

        //Events
        if (Input.GetKeyDown(StartMainMusic))
        {
            AudioManager.Instance.PostWwiseEventGlobal(AudioManager.Instance.MainMusic_Play);
        }
        if (Input.GetKeyDown(StopMainMusic))
        {
            AudioManager.Instance.PostWwiseEventGlobal(AudioManager.Instance.MainMusic_Stop);
        }


        // Game States
        if (Input.GetKeyDown(Game_GameOver))
        {
            AudioManager.Instance.SetAudioGameState(AudioGameState.GameOver);
        }
        if (Input.GetKeyDown(Game_Gameplay))
        {
            AudioManager.Instance.SetAudioGameState(AudioGameState.Gameplay);
        }
        if (Input.GetKeyDown(Game_Paused))
        {
            AudioManager.Instance.SetAudioGameState(AudioGameState.Paused);
        }
        //Music States
        if (Input.GetKeyDown(Music_MainMenu))
        {
            AudioManager.Instance.SetAudioMusicState(AudioMusicState.MainMenu);
        }
        if (Input.GetKeyDown(Music_LevelStart))
        {
            AudioManager.Instance.SetAudioMusicState(AudioMusicState.GameplayFirstSection);
        }
        if (Input.GetKeyDown(Music_LevelWin))
        {
            AudioManager.Instance.SetAudioMusicState(AudioMusicState.LevelWin);
        }
        if (Input.GetKeyDown(Music_LevelLose))
        {
            AudioManager.Instance.SetAudioMusicState(AudioMusicState.LevelLose);
        }

    }


}
