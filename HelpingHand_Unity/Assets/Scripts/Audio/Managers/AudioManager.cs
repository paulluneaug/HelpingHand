using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

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
        EventManager.RoomMachinist_Ambience_Play.Post(gameObject);
        EventManager.Theater_Ambience_Play.Post(gameObject);
    }

    #region Functions
    #region PostWwise Events

    public async UniTask PostWwiseEventAsync(WwiseEvent wwiseEvent, GameObject targetObject = null, CancellationToken cancellationToken = default)
    {
        if (wwiseEvent == null)
        {
            Debug.LogError("wwiseEvent is null (check if it's set correctly and up to date :)");
            return;
        }

        if (!wwiseEvent.IsValid())
        {
            Debug.LogError(wwiseEvent.Name + " is invalid, check if it's set correctly and up to date");
        }

        bool isEnded = false;
        uint playingID = wwiseEvent.Post(targetObject, (uint)AkCallbackType.AK_EndOfEvent, (inCookie, inType, inInfo) =>
        {
            isEnded = true;
        }, null);
        
        if (await UniTask.WaitUntil(() => isEnded, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
        {
            Debug.Log($"[{Time.frameCount}] [{nameof(AudioManager)}] PostWwiseEventToObjectAsync interrupted");
            AkUnitySoundEngine.StopPlayingID(playingID);
            throw new OperationCanceledException();
        }
        
        Debug.Log($"[{Time.frameCount}] [{nameof(AudioManager)}] PostWwiseEventToObjectAsync end");
    }
    
    #endregion
    #region Play UI Sounds
    public void PlayTypewriter(GameObject targetObject)
    {
        EventManager.Typewriter_Play.Post(targetObject);
    }

    public void PlayButton(GameObject targetObject)
    {
        EventManager.ButtonOnPointerDown_Play.Post(targetObject);
    }
    
    public void ToggleSound(bool isOn, GameObject targetObject)
    {
        if (isOn)
        {
            EventManager.Toggle_Play.Post(targetObject);
        }
        else
        {
            EventManager.Untoggle_Play.Post(targetObject);
        }
    }
    #endregion

    public async UniTask PlayDialogueWithStatesAsync(string repetition, string etat, string objet, string narra, GameObject targetObject = null, CancellationToken cancellationToken = default)
    {        
        Debug.Log($"[{Time.frameCount}] [{nameof(AudioManager)}] PlayDialogueWithStatesAsync {repetition}, {etat}, {objet}, {narra}");

        uint dialogueEventId = AkUnitySoundEngine.GetIDFromString("Dialogue_Event");
        // Pour référencer le dialogue event : pas possible de faire une variable comme un event classique
        // Il faut référencer l'ID stocké dans la soundbank

        if (dialogueEventId == AkUnitySoundEngine.AK_INVALID_UNIQUE_ID)
        {
            Debug.LogError("L'event 'Dialogue_Event' est introuvable. Vérifier son nom et sa SoundBank.");
            return;
        }

        // Récupération des State IDs depuis les noms
        uint repetitionID = AkUnitySoundEngine.GetIDFromString(repetition);
        uint etatID = AkUnitySoundEngine.GetIDFromString(etat);
        uint objetID = AkUnitySoundEngine.GetIDFromString(objet);
        uint narraID = AkUnitySoundEngine.GetIDFromString(narra);
        //Rajouter des states ici si on en a besoin de + !

        if (repetitionID == 0 || etatID == 0 || objetID == 0)
        {
            Debug.LogError($"Échec de conversion des states: {repetition}, {etat}, {objet}, {narra}");
            return;
        }

        uint[] args = new uint[] { etatID, objetID, repetitionID, narraID };

        bool isEnded = false;
        uint sequenceID = AkUnitySoundEngine.DynamicSequenceOpen(gameObject, (uint)AkCallbackType.AK_EndOfDynamicSequenceItem, (inCookie, inType, inInfo) =>
        {
            isEnded = true;
        }, null);
        
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

        if (await UniTask.WaitUntil(() => isEnded, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
        {
            Debug.Log($"[{Time.frameCount}] [{nameof(AudioManager)}] PlayDialogueWithStatesAsync interrupted");
            AkUnitySoundEngine.DynamicSequenceStop(sequenceID);
            AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
            throw new OperationCanceledException();
        }
        
        AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
        Debug.Log($"[{Time.frameCount}] [{nameof(AudioManager)}] PlayDialogueWithStatesAsync end");
    }

    #endregion
}
