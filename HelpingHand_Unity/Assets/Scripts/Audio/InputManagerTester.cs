using UnityEngine;

public class InputManagerTester : MonoBehaviour
{
    public static InputManagerTester Instance;
    private bool m_bIsInitialized = false;
    [Header("Event Input Key Assignments")]
    [SerializeField] private KeyCode m_startMainMusic;
    [SerializeField] private KeyCode m_stopMainMusic;

    [Header("GameState Input Key Assignments")]
    [SerializeField] private KeyCode m_game_GameOver;
    [SerializeField] private KeyCode m_game_Gameplay;
    [SerializeField] private KeyCode m_game_Paused;

    [Header("MusicState Input Key Assignments")]
    [SerializeField] private KeyCode m_music_MainMenu;
    [SerializeField] private KeyCode m_music_LevelStart;
    [SerializeField] private KeyCode m_music_LevelWin;
    [SerializeField] private KeyCode m_music_PauseMenu;
    [SerializeField] private KeyCode m_music_LevelLose;

    private void Awake()
    {
        Initialize();
    }
    private void Initialize()
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
        {
            return;
        }

        //Events
        if (Input.GetKeyDown(m_startMainMusic))
        {
            //AudioManager.Instance.PostWwiseEventGlobal(AudioManager.Instance.MainMusic_Play);
        }
        if (Input.GetKeyDown(m_stopMainMusic))
        {
            //AudioManager.Instance.PostWwiseEventGlobal(AudioManager.Instance.MainMusic_Stop);
        }


        // Game States
        if (Input.GetKeyDown(m_game_GameOver))
        {
            //AudioManager.Instance.SetAudioGameState(AudioGameState.GameOver);
        }
        if (Input.GetKeyDown(m_game_Gameplay))
        {
            //AudioManager.Instance.SetAudioGameState(AudioGameState.Gameplay);
        }
        if (Input.GetKeyDown(m_game_Paused))
        {
            //AudioManager.Instance.SetAudioGameState(AudioGameState.Paused);
        }
        //Music States
        if (Input.GetKeyDown(m_music_MainMenu))
        {
            // AudioManager.Instance.SetAudioMusicState(AudioMusicState.MainMenu);
        }
        if (Input.GetKeyDown(m_music_LevelStart))
        {
            //  AudioManager.Instance.SetAudioMusicState(AudioMusicState.GameplayFirstSection);
        }
        if (Input.GetKeyDown(m_music_LevelWin))
        {
            //  AudioManager.Instance.SetAudioMusicState(AudioMusicState.LevelWin);
        }
        if (Input.GetKeyDown(m_music_LevelLose))
        {
            //  AudioManager.Instance.SetAudioMusicState(AudioMusicState.LevelLose);
        }

    }


}
