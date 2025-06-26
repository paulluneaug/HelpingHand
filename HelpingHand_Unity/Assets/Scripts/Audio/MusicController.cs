using System;

using Sirenix.OdinInspector;

using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Serializable]
    private class MusicStates
    {
        [SerializeField] private EntityState m_musicPlayingState;
        [SerializeField] private EntityState m_musicPlayedState;
        [SerializeField] private MusicState m_musicState;

        public void SetIsPlaying(bool isPlaying)
        {
            m_musicPlayingState.Value = isPlaying;
        }

        public void Play()
        {
            AudioManager.Instance.StateManager.SetMusicState(m_musicState);
        }
    }

    [Title("Input Events")]
    [SerializeField] private RotaryEncoderInputEvent m_selectMusicInput;
    [SerializeField] private BaseVariable<bool> m_selectedMusic0;
    [SerializeField] private BaseVariable<bool> m_selectedMusic1;
    [SerializeField] private ButtonInputEvent m_playMusicEvent;

    [Title("Entity states")]
    [SerializeField, RequiredListLength(MinLength = 4, MaxLength = 4)] private MusicStates[] m_states = new MusicStates[4];

    // Cache
    [NonSerialized] private int m_selectedMusicIndex = -1;
    [NonSerialized] private int m_playingMusicIndex = -1;
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

        if (GameManager.ApplicationIsQuitting)
        {
            return;
        }
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
        m_selectMusicInput.AddIndexListener(OnIndexChanged);
        m_playMusicEvent.AddDownListener(OnPlayMusicChanged);

        m_selectedMusicIndex = m_selectMusicInput.Index.Value.Mod(m_states.Length);
    }

    private void Dispose()
    {
        m_selectMusicInput.RemoveIndexListener(OnIndexChanged);
        m_playMusicEvent.RemoveListener(OnPlayMusicChanged);
    }

    private void OnPlayMusicChanged()
    {
        // Button pressed on the same music => stop the music
        if (m_selectedMusicIndex == m_playingMusicIndex)
        {
            Debug.Log($"Stopping music at index {m_selectedMusicIndex}");
            m_states[m_selectedMusicIndex].SetIsPlaying(false);
            m_playingMusicIndex = -1;
            AudioManager.Instance.StateManager.SetMusicState(MusicState.None);
        }
        else
        {
            // Else, start the new music
            if (m_playingMusicIndex != -1)
            {
                m_states[m_playingMusicIndex].SetIsPlaying(false);
            }
            Debug.Log($"Playing music at index {m_selectedMusicIndex}");
            m_states[m_selectedMusicIndex].SetIsPlaying(true);
            m_playingMusicIndex = m_selectedMusicIndex;
            m_states[m_selectedMusicIndex].Play();
        }
    }

    private void OnIndexChanged(int index)
    {
        m_selectedMusicIndex = index.Mod(m_states.Length);
    }
}
