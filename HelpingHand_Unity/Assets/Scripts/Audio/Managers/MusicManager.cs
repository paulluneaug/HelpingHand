using UnityEngine;

using UnityUtility.CustomAttributes;

using WwiseRTPC = AK.Wwise.RTPC;
using WwiseEvent = AK.Wwise.Event;
using WwiseState = AK.Wwise.State;

public class MusicManager : MonoBehaviour
{
    #region GameState variables
    [Title("Game States")]
    [SerializeField, Disable] private AudioGameState m_currentGameState;

    [Title("Music States")]
    [SerializeField] private WwiseState m_music_Gameplay1stSection;
    [SerializeField] private WwiseState m_music_Gameplay2ndSection;
    [SerializeField] private WwiseState m_music_Gameplay3rdSection;
    [SerializeField] private WwiseState m_music_MainMenu;
    [SerializeField] private WwiseState m_music_Level_Lose;
    [SerializeField] private WwiseState m_music_Level_Win;
    [SerializeField] private WwiseState m_music_None;

    [Title("RTPC")]
    [SerializeField, Range(0f, 1f)] private float m_music_FirstLayer = 0;
    [SerializeField, Range(0f, 1f)] private float m_music_SecondLayer = 0;


    [SerializeField, Disable] private AudioMusicState m_currentMusicState;
    #endregion

    #region Sound Events
    [Title("Wwise Music Events")]
    [SerializeField] private WwiseEvent m_mainMusic_Play;
    [SerializeField] private WwiseEvent m_mainMusic_Stop;

    [Title("Wwise RTPC")]
    [SerializeField] private WwiseRTPC m_music_FirstLayer_RTPC;
    [SerializeField] private WwiseRTPC m_music_SecondLayerRTPC;
    #endregion

    private void Awake()
    {
        SetAudioMusicState(AudioMusicState.None); //On initialise l'état de la musique à None (reset)  
    }

    private void Start()
    {
        SetAudioMusicState(AudioMusicState.MainMenu); //On initialise l'état de la musique à MainMenu

        _ = m_mainMusic_Play.Post(gameObject); //On joue la musique principale

        m_music_FirstLayer_RTPC.SetGlobalValue(m_music_FirstLayer);
        m_music_SecondLayerRTPC.SetGlobalValue(m_music_SecondLayer);
    }

    private void Update()
    {
        m_music_FirstLayer_RTPC.SetGlobalValue(m_music_FirstLayer);
        m_music_SecondLayerRTPC.SetGlobalValue(m_music_SecondLayer);
    }

    public void SetAudioMusicState(AudioMusicState MusicState) // Change l'état des states liés à la musique
    {
        if (MusicState == m_currentMusicState)
        {
            Debug.Log("MusicState is already" + MusicState + "."); //On ne change pas l'état si c'est déjà le bon
            return;
        }

        switch (MusicState)
        {
            default: //Cas pas défaut = mainmenu
            case AudioMusicState.MainMenu:
                m_music_MainMenu.SetValue();
                break;
            case AudioMusicState.GameplayFirstSection:
                m_music_Gameplay1stSection.SetValue();
                break;
            case AudioMusicState.LevelLose:
                m_music_Level_Lose.SetValue();
                break;
            case AudioMusicState.None:
                m_music_None.SetValue();
                break;
            case AudioMusicState.GameplaySecondSection:
                break;
            case AudioMusicState.GameplayThirdSection:
                break;
            case AudioMusicState.PauseMenu:
                break;
            case AudioMusicState.LevelWin:
                break;
        }
        m_currentMusicState = MusicState; //On met à jour l'état actuel
        Debug.Log("New Wwise GameState: " + MusicState + ".");
    }

    public void PostWwiseEventGlobal(WwiseEvent WwiseEvent)
    {
        if (WwiseEvent == null)
        {
            Debug.LogError(WwiseEvent.Name + " is null (check if it's set correctly and up to date :)");
            return;
        }

        if (WwiseEvent.IsValid())
        {
            _ = WwiseEvent.Post(gameObject);
        }
        else
        {
            Debug.LogError(WwiseEvent.Name + " is invalid, check if it's set correctly and up to date");
        }
    }

    public void PostWwiseEventToObject(WwiseEvent WwiseEvent, GameObject TargetObject)
    {
        if (WwiseEvent == null)
        {
            Debug.LogError(WwiseEvent.Name + " is null (check if it's set correctly and up to date :)");
            return;
        }
        else if (TargetObject == null)
        {
            Debug.LogError(TargetObject.name + " is null. PostWwiseEventToObject requires an existing TargetObject.");
            return;
        }

        if (WwiseEvent.IsValid())
        {
            _ = WwiseEvent.Post(TargetObject);
        }
        else
        {
            Debug.LogError(WwiseEvent.Name + " is invalid, check if it's set correctly and up to date");
        }


    }


}
