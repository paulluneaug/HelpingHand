using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

using Title = UnityUtility.CustomAttributes.TitleAttribute;
using TitleAlignments = Sirenix.OdinInspector.TitleAlignments;

using WwiseEvent = AK.Wwise.Event;
using WwiseRTPC = AK.Wwise.RTPC;
using WwiseState = AK.Wwise.State;

public enum AudioGameState
{
    None,
    MainMenu,
    Gameplay,
    Paused,
    GameOver
}
public enum AudioMusicState
{
    None,
    MainMenu,
    GameplayFirstSection,
    GameplaySecondSection,
    GameplayThirdSection,
    PauseMenu,
    LevelWin,
    LevelLose
}
public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    #region Accessors
    [field: Title("Sub Managers")]
    [field: SerializeField]
    public MusicManager MusicManager { get; }
    #endregion


    #region Sound Events
    [TitleGroup("Events", horizontalLine: true, alignment: TitleAlignments.Centered, boldTitle: true, indent: true)]
    public string EventExplanation;
    //Music
    [SerializeField] public WwiseEvent MainMusic_Play;
    [SerializeField] public WwiseEvent MainMusic_Stop;
    //SFX
    [SerializeField] public WwiseEvent Footsteps;
    [SerializeField] public WwiseEvent Block_translation;
    // UI
    [SerializeField] public WwiseEvent UI_Click;
    [SerializeField] public WwiseEvent UI_Hover;

    [SerializeField] public WwiseEvent MenuOpenSound;
    [SerializeField] public WwiseEvent MenuCloseSound;
    #endregion

    #region Soundbanks list
    [TitleGroup("Startup SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [SerializeField] private List<AK.Wwise.Bank> Soundbanks;
    #endregion

    #region GameState variables
    [TitleGroup("States", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    public string GameStates;

    [HorizontalGroup("Line1", Width = 0.4f)]
    [SerializeField] private WwiseState Game_Paused;
    [HorizontalGroup("Line1", Width = 0.4f)]
    [SerializeField] private WwiseState Game_MainMenu;
    [HorizontalGroup("Line2", Width = 0.4f)]
    [SerializeField] private WwiseState Game_None;
    [HorizontalGroup("Line2", Width = 0.4f)]
    [SerializeField] private WwiseState Game_GameOver;
    [HorizontalGroup("Line3", Width = 0.5f)]
    [SerializeField] private WwiseState Game_Gameplay;
    [HorizontalGroup("Line3", Width = 0.5f)]
    [SerializeField] private WwiseState Game_Win;
    [Disable][SerializeField] private AudioGameState currentGameState;
    [HorizontalGroup("Line4", Width = 0.5f)]
    [SerializeField] private WwiseState Music_Gameplay1stSection;
    [HorizontalGroup("Line5", Width = 0.5f)]
    [SerializeField] private WwiseState Music_Gameplay2ndSection;
    [HorizontalGroup("Line6", Width = 0.5f)]
    [SerializeField] private WwiseState Music_Gameplay3rdSection;

    [HorizontalGroup("Line4", Width = 0.5f)]
    [SerializeField] private WwiseState Music_MainMenu;
    [HorizontalGroup("Line5", Width = 0.5f)]
    [SerializeField] private WwiseState Music_Level_Lose;
    [HorizontalGroup("Line6", Width = 0.5f)]
    [SerializeField] private WwiseState Music_None;

    [Disable][SerializeField] private AudioMusicState currentMusicState;
    #endregion


    #region RTPC
    [TitleGroup("RTPC", horizontalLine: true, alignment: TitleAlignments.Centered, boldTitle: true, indent: true)]
    public string RTPC;
    [HorizontalGroup("Line7", Width = 0.25f)]
    [SerializeField][Range(0f, 1f)] private float Music1Layer1 = 0;
    [HorizontalGroup("Line7", Width = 0.25f)]
    [SerializeField][Range(0f, 1f)] private float Music1Layer2 = 0;
    [HorizontalGroup("Line7", Width = 0.25f)]
    [SerializeField][Range(0f, 1f)] private float Music1Layer3 = 0;
    [HorizontalGroup("Line7", Width = 0.25f)]
    [SerializeField][Range(0f, 1f)] private float Music1Layer4 = 0;
    [HorizontalGroup("Line8")]
    public WwiseRTPC RTPC_Music_FirstLayer;
    [HorizontalGroup("Line8")]
    public WwiseRTPC RTPC_Music_secondLayer;

    #endregion


    #region Switches
    [Header("Switches")]
    [SerializeField] private AK.Wwise.Switch Material;
    #endregion


    public override void Initialize()
    {
        base.Initialize();

        SetAudioGameState(AudioGameState.None); //On initialise l'état du jeu à None (reset)
        SetAudioMusicState(AudioMusicState.None); //On initialise l'état de la musique à None (reset)  
    }

    private void Start()
    {
        SetAudioGameState(AudioGameState.MainMenu); //On initialise l'état du jeu à MainMenu
        SetAudioMusicState(AudioMusicState.MainMenu); //On initialise l'état de la musique à MainMenu

        _ = MainMusic_Play.Post(gameObject); //On joue la musique principale

        RTPC_Music_FirstLayer.SetValue(null, Music1Layer1);
        RTPC_Music_secondLayer.SetValue(null, Music1Layer2);
    }

    private void Update()
    {
        RTPC_Music_FirstLayer.SetValue(null, Music1Layer1);
        RTPC_Music_secondLayer.SetValue(null, Music1Layer2);
    }

    private void LoadSoundbanks() //Load les soundbanks (pas encore dynamiquement)
    {
        if (Soundbanks.Count > 0) // Dans le cas où l'on a des soundbanks
        {
            foreach (AK.Wwise.Bank bank in Soundbanks) //Load toutes les soundbanks dans la liste
            {
                bank.Load();
            }
        }
        else
        {
            Debug.LogError("No SoundBanks found in the list. Please add soundbanks to the Audiomanager :)");
        }
    }

    public void SetAudioGameState(AudioGameState GameState) // Change l'état des states liés au jeu
    {
        if (GameState == currentGameState) //Si c'est la même valeur que celle actuelle, on ne fait rien
        {
            Debug.Log("GameState is already" + GameState + "."); //On ne change pas l'état si c'est déjà le bon
            return;
        }
        switch (GameState) // On change l'état en fonction de l'état du jeu
        {
            default: //Cas pas défaut = mainmenu
            case AudioGameState.MainMenu:
                Game_MainMenu.SetValue();
                break;
            case AudioGameState.Gameplay:
                Game_Gameplay.SetValue();
                break;
            case AudioGameState.Paused:
                Game_Paused.SetValue();
                break;
            case AudioGameState.GameOver:
                Game_GameOver.SetValue();
                break;
            case AudioGameState.None:
                Game_None.SetValue();
                break;
        }
        Debug.Log("New Wwise GameState: " + GameState + "."); //On affiche le nouvel état dans la console

        currentGameState = GameState; //On met à jour l'état actuel
    }

    public void SetAudioMusicState(AudioMusicState MusicState) // Change l'état des states liés à la musique
    {
        if (MusicState == currentMusicState)
        {
            Debug.Log("MusicState is already" + MusicState + "."); //On ne change pas l'état si c'est déjà le bon
            return;
        }

        switch (MusicState)
        {
            default: //Cas pas défaut = mainmenu
            case AudioMusicState.MainMenu:
                Music_MainMenu.SetValue();
                break;
            case AudioMusicState.GameplayFirstSection:
                Music_Gameplay1stSection.SetValue();
                break;
            case AudioMusicState.LevelLose:
                Music_Level_Lose.SetValue();
                break;
            case AudioMusicState.None:
                Music_None.SetValue();
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
        currentMusicState = MusicState; //On met à jour l'état actuel
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
