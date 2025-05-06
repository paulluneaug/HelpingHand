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

    public override void Initialize()
    {
        SoundbankManager.LoadStartupSoundbanks(); //Charge les soundbanks de début

        RTPCManager.InitRtpcDictionaries(); //Initialise les RTPC
        SwitchManager.InitSwitchDictionaries(); //Initialise les switchs
        StateManager.SetGameState(GameState.None); //On initialise l'état du jeu à None (reset)
        StateManager.SetMusicState(MusicState.None); //On initialise l'état de la musique à None (reset)
    }

    protected override void Start()
    {
        StateManager.SetGameState(GameState.MainMenu); //On initialise l'état du jeu à MainMenu
        StateManager.SetMusicState(MusicState.MainMenu); //On initialise l'état de la musique à MainMenu

        RTPCManager.FirstMusic_FirstLayer.SetValue(null, 0);
        RTPCManager.FirstMusic_SecondLayer.SetValue(null, 0);

        //On joue la musique principale dès que la scène se lance
       // _ = EventManager.MainMusic_Play.Post(gameObject);

        // Ambiances de pièces qui se jouent dès le début
        _ = EventManager.RoomMachinist_Ambience_Play.Post(null);
        _ = EventManager.Theater_Ambience_Play.Post(null);
    }

    #region Functions
    #region PostWwise Events
    public void PostWwiseEventGlobal(WwiseEvent WwiseEvent)
    {
        if (WwiseEvent == null)
        {
            Debug.LogError(WwiseEvent.Name + " is null (check if it's set correctly and up to date :)");
            return;
        }

        if (WwiseEvent.IsValid())
        {
            _ = WwiseEvent.Post(null);
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
    #region Play UI Sounds
    public void PlayTypewriter(GameObject targetObject)
    {
        _ = EventManager.Typewriter_Play.Post(targetObject);
    }

    public void PlayButton(GameObject targetObject)
    {
        _ = EventManager.ButtonOnPointerDown_Play.Post(targetObject);
    }
    public void ToggleSound(bool isOn, GameObject targetObject)
    {
        if (isOn)
        {
            _ = EventManager.Toggle_Play.Post(targetObject);
        }
        else
        {
            _ = EventManager.Untoggle_Play.Post(targetObject);
        }
    }
    #endregion

    public void PlayDialogueWithStates(string repetition, string etat, string objet)
    {
        uint dialogueEventId = AkUnitySoundEngine.GetIDFromString("DirectorVoice");
        // Pour référencer le dialogue event : pas possible de faire une variable comme un event classique
        // Il faut référencer l'ID stocké dans la soundbank

        if (dialogueEventId == AkUnitySoundEngine.AK_INVALID_UNIQUE_ID)
        {
            Debug.LogError("L'event 'DirectorVoice' est introuvable. Vérifier son nom et sa SoundBank.");
            return;
        }

        // Récupération des State IDs depuis les noms
        uint repetitionID = AkUnitySoundEngine.GetIDFromString(repetition);
        uint etatID = AkUnitySoundEngine.GetIDFromString(etat);
        uint objetID = AkUnitySoundEngine.GetIDFromString(objet);
        //Rajouter un state ici si nécessaire


        if (repetitionID == 0 || etatID == 0 || objetID == 0)
        {
            Debug.LogError($"Échec de conversion des states: {repetition}, {etat}, {objet}");
            return;
        }

        uint[] args = new uint[] { etatID, objetID, repetitionID };

        uint sequenceID = AkUnitySoundEngine.DynamicSequenceOpen(this.gameObject);
        AkPlaylist playlist = AkUnitySoundEngine.DynamicSequenceLockPlaylist(sequenceID);

        uint nodeID = AkUnitySoundEngine.ResolveDialogueEvent(dialogueEventId, args, (uint)args.Length);
        if (nodeID == AkUnitySoundEngine.AK_INVALID_UNIQUE_ID)
        {
            Debug.LogError("Aucun dialogue node trouvé pour ces states.");
            AkUnitySoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
            AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
            return;
        }

        playlist.Enqueue(nodeID);
        AkUnitySoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        AkUnitySoundEngine.DynamicSequencePlay(sequenceID);
        AkUnitySoundEngine.DynamicSequenceClose(sequenceID);

        Debug.Log($"Dialogue joué avec : Etat={etat}, Objet={objet}, Repetition={repetition}");

        //Note : si on veut arrêter le son on utilise DynamicSequenceStop(sequenceID) puis DynamicSequenceClose(sequenceID)
        //Note : si on veut reprendre où on en était, on utilise DynamicSequenceResume(sequenceID)
         
    }



    #endregion
}
