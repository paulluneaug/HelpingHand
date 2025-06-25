using System;

using Sirenix.OdinInspector;

using UnityEngine;

using WwiseEvent = AK.Wwise.Event;

public class MusicController : MonoBehaviour
{
    [Serializable]
    private class MusicStates
    {
        [SerializeField] private EntityState m_musicPlayingState;
        [SerializeField] private EntityState m_musicPlayedState;

        [SerializeField] private WwiseEvent m_switchToMusic;

        public void SetIsPlaying(bool isPlaying)
        {
            m_musicPlayingState.Value = isPlaying;
        }

        public void SwitchToMusic(GameObject go)
        {
            _ = m_switchToMusic.Post(go);
        }

    }

    [Title("Input Events")]
    [SerializeField] private BaseVariable<bool> m_selectedMusic0;
    [SerializeField] private BaseVariable<bool> m_selectedMusic1;
    [SerializeField] private ButtonInputEvent m_playMusicEvent;

    [Title("Entity states")]
    [SerializeField, RequiredListLength(MinLength = 4, MaxLength = 4)] private MusicStates[] m_states = new MusicStates[4];

    [Title("Wwise Events")]
    [SerializeField] private WwiseEvent m_startMusic;
    [SerializeField] private WwiseEvent m_stopMusic;

    // Cache
    [NonSerialized] private int m_selectedMusic = 0;
    [NonSerialized] private bool m_playingMusic = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        OnGameStateChanged(GameManager.Instance.CurrentGameState);
    }

    private void OnDestroy()
    {
        Dispose();
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        if (state != GameManager.GameState.Gameplay)
        {
            return;
        }

        Init();

    }

    private void Init()
    {
        m_selectedMusic0.AddListener(OnSelectedMusic0Changed);
        m_selectedMusic1.AddListener(OnSelectedMusic1Changed);
        m_playMusicEvent.AddDownListener(OnPlayMusicChanged);

        UpdateSelectedMusic();
    }

    private void Dispose()
    {
        m_selectedMusic0.RemoveListener(OnSelectedMusic0Changed);
        m_selectedMusic1.RemoveListener(OnSelectedMusic1Changed);
        m_playMusicEvent.RemoveListener(OnPlayMusicChanged);
    }

    private void OnPlayMusicChanged()
    {
        m_playingMusic = !m_playingMusic;
        if (m_playingMusic) 
        {

            _ = m_startMusic.Post(gameObject);
            m_states[m_selectedMusic].SetIsPlaying(true);
        }
        else
        {
            _ = m_stopMusic.Post(gameObject);
            m_states[m_selectedMusic].SetIsPlaying(false);
        }
    }

    private void OnSelectedMusic1Changed(bool obj)
    {
        UpdateSelectedMusic();
    }

    private void OnSelectedMusic0Changed(bool obj)
    {
        UpdateSelectedMusic();
    }

    private void UpdateSelectedMusic()
    {
        int newSelectedMusic = 0;
        newSelectedMusic |= (m_selectedMusic0.Value ? (1 << 0) : 0);
        newSelectedMusic |= (m_selectedMusic1.Value ? (1 << 1) : 0);

        if (newSelectedMusic == m_selectedMusic)
        {
            return;
        }

        m_selectedMusic = newSelectedMusic;
        // Stop music
        _ = m_stopMusic.Post(gameObject);
        m_states[m_selectedMusic].SetIsPlaying(false);

        m_playingMusic = false;

        m_states[m_selectedMusic].SwitchToMusic(gameObject);
    }

}
