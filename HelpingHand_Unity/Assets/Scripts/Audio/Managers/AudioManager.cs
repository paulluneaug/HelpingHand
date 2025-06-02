using System;
using System.Diagnostics;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

using Debug = UnityEngine.Debug;
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
        // EventManager.MainMusic_Play.Post(gameObject);

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
            DebugLog($"wwiseEvent is null (check if it's set correctly and up to date :)", LogType.Error);
            return;
        }

        if (!wwiseEvent.IsValid())
        {
            DebugLog($"{wwiseEvent.Name} is invalid, check if it's set correctly and up to date", LogType.Error);
        }

        bool isEnded = false;
        uint playingID = wwiseEvent.Post(targetObject, (uint)AkCallbackType.AK_EndOfEvent, (inCookie, inType, inInfo) =>
        {
            isEnded = true;
        }, null);

        if (await UniTask.WaitUntil(() => isEnded, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
        {
            DebugLog($"PostWwiseEventToObjectAsync interrupted");
            AkUnitySoundEngine.StopPlayingID(playingID);
            throw new OperationCanceledException();
        }

        DebugLog($"PostWwiseEventToObjectAsync end");
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
        DebugLog($"PlayDialogueWithStatesAsync {repetition}, {etat}, {objet}, {narra}");

        uint dialogueEventId = AkUnitySoundEngine.GetIDFromString("Dialogue_Event");
        // Pour référencer le dialogue event : pas possible de faire une variable comme un event classique
        // Il faut référencer l'ID stocké dans la soundbank

        if (dialogueEventId == AkUnitySoundEngine.AK_INVALID_UNIQUE_ID)
        {
            DebugLog($"L'event 'Dialogue_Event' est introuvable. Vérifier son nom et sa SoundBank.", LogType.Error);
            return;
        }

        // Récupération des State IDs depuis les noms
        uint repetitionID = AkUnitySoundEngine.GetIDFromString(repetition);
        uint etatID = AkUnitySoundEngine.GetIDFromString(etat);
        uint objetID = AkUnitySoundEngine.GetIDFromString(objet);
        uint narraID = AkUnitySoundEngine.GetIDFromString(narra);
        // TODO ajouter le ton du narrateur
        //Rajouter des states ici si on en a besoin de + !

        if (repetitionID == 0 || etatID == 0 || objetID == 0)
        {
            DebugLog($"Échec de conversion des states: {repetition}, {etat}, {objet}, {narra}", LogType.Error);
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
            DebugLog($"Aucun dialogue node trouvé pour ces states.", LogType.Error);
            AkUnitySoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
            AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
            return;
        }

        playlist.Enqueue(nodeID);
        AkUnitySoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        AkUnitySoundEngine.DynamicSequencePlay(sequenceID);

        if (await UniTask.WaitUntil(() => isEnded, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
        {
            DebugLog($"PlayDialogueWithStatesAsync interrupted");
            AkUnitySoundEngine.DynamicSequenceStop(sequenceID);
            AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
            throw new OperationCanceledException();
        }

        AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
        DebugLog($"PlayDialogueWithStatesAsync end");
    }

    #endregion

    /// <summary>
    /// Debug log with header
    /// TODO: move it project-wise 
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    private void DebugLog(string log, LogType logType = LogType.Log, GameObject source = null)
    {
        string GetLogHeader()
        {
            return $"[{Time.frameCount}] <color=green>[{nameof(AudioManager)}]</color>";
        }
        
        switch (logType)
        {
            case LogType.Error:
                Debug.LogError($"{GetLogHeader()} {log}", source);
                break;
            case LogType.Warning:
                Debug.LogWarning($"{GetLogHeader()} {log}", source);
                break;
            case LogType.Log:
                Debug.Log($"{GetLogHeader()} {log}", source);
                break;
        }
    }
}
