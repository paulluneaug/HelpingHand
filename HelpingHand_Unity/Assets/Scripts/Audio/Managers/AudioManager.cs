using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;
using Title = UnityUtility.CustomAttributes.TitleAttribute;
using WwiseEvent = AK.Wwise.Event;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    #region Accessors
    [field: Title("Sub Managers")]
  //  [field: SerializeField]
    public SwitchManager SwitchManager;
    public RTPCManager RTPCManager;
    public StateManager StateManager;
    public EventManager EventManager;
    public SoundbankManager SoundbankManager;
    #endregion
    public new void Awake()
    {
        SoundbankManager.LoadStartupSoundbanks(); //Charge les soundbanks de début

        RTPCManager.InitRtpcDictionaries(); //Initialise les RTPC
        SwitchManager.InitSwitchDictionaries(); //Initialise les switchs
        StateManager.SetGameState(GameState.None); //On initialise l'état du jeu à None (reset)
        StateManager.SetMusicState(MusicState.None); //On initialise l'état de la musique à None (reset)
    }

    public new void Start()
    {
        StateManager.SetGameState(GameState.MainMenu); //On initialise l'état du jeu à MainMenu
        StateManager.SetMusicState(MusicState.MainMenu); //On initialise l'état de la musique à MainMenu

        RTPCManager.FirstMusic_FirstLayer.SetValue(null, 0);
        RTPCManager.FirstMusic_SecondLayer.SetValue(null, 0);

        //On joue la musique principale dès que la scène se lance
        _ = EventManager.MainMusic_Play.Post(gameObject);

        // Ambiances de pièces qui se jouent dès le début
        _ = EventManager.RoomMachinist_Ambience_Play.Post(gameObject);
        _ = EventManager.Theater_Ambience_Play.Post(gameObject);
    }

    #region Functions
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
    #endregion


}
