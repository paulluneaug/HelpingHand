using System;
using System.Diagnostics;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

using static RTPCManager;

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

    [SerializeField] private InputAudioEventControllersManager m_inputAudioEventsManager;
    #endregion

    public override void Initialize()
    {
        _ = AkUnitySoundEngine.RegisterGameObj(gameObject, "AudioManager");
        //RTPCManager.InitRtpcDictionaries(); //Initialise les RTPC
        SwitchManager.InitSwitchDictionaries(); //Initialise les switchs
        StateManager.SetGameState(GameState.None); //On initialise l'état du jeu à None (reset)
        StateManager.SetMusicState(MusicState.None); //On initialise l'état de la musique à None (reset)
        m_inputAudioEventsManager.Init();
    }

    protected override void Start()
    {

        SoundbankManager.LoadStartupSoundbanks(); //Charge les soundbanks de début

        StateManager.SetGameState(GameState.MainMenu); //On initialise l'état du jeu à MainMenu
        StateManager.SetMusicState(MusicState.MainMenu); //On initialise l'état de la musique à MainMenu

        RTPCManager.FirstMusic_FirstLayer.SetValue(null, 0);
        RTPCManager.FirstMusic_SecondLayer.SetValue(null, 0);

        //On joue la musique principale dès que la scène se lance
        //EventManager.MainMusic_Play.Post(gameObject);

        // Ambiances de pièces qui se jouent dès le début
        _ = EventManager.RoomMachinist_Ambience_Play.Post(gameObject);
        _ = EventManager.Theater_Ambience_Play.Post(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _ = AkUnitySoundEngine.UnregisterGameObj(gameObject);

        m_inputAudioEventsManager.Dispose();
    }

    #region Functions

    public void SetVolume(BusRtpcType group, float value)
    {
        //float wwiseVolume = Mathf.Clamp(value, 0f, 100f);
        switch (group)
        {
            case BusRtpcType.Master:
                RTPCManager.RTPC_MasterVolume.SetValue(null, value);
                break;
            case BusRtpcType.Music:
                RTPCManager.RTPC_MusicVolume.SetValue(null, value);
                break;
            case BusRtpcType.SFX:
                RTPCManager.RTPC_SFXVolume.SetValue(null, value);
                break;
            case BusRtpcType.Voice:
                RTPCManager.RTPC_VoiceVolume.SetValue(null, value);
                break;
            case BusRtpcType.UI:
                break;
            default:
                break;
        }
    }

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

    public async UniTask PlayDialogueWithStatesAsync(
        string onboardingIntro,
        string onboardingCurtain,
        string onboardingSpots,
        string interruptionCurtain,
        string interruptionSpots,
        string roueState,
        string combatState,
        GameObject targetObject = null,
        CancellationToken cancellationToken = default)
    {
        DebugLog($"PlayDialogueWithStatesAsync: {onboardingIntro}, {onboardingCurtain}, {onboardingSpots}, {interruptionCurtain}, {interruptionSpots}, {roueState}, {combatState}");
        uint dialogueEventId = AkUnitySoundEngine.GetIDFromString("Dialogue_Event");
        // Pour référencer le dialogue event : pas possible de faire une variable comme un event classique
        // Il faut référencer l'ID stocké dans la soundbank

        if (dialogueEventId == AkUnitySoundEngine.AK_INVALID_UNIQUE_ID)
        {
            DebugLog($"L'event 'Dialogue_Event' est introuvable. Vérifier son nom et sa SoundBank.", LogType.Error);
            return;
        }

        // Récupération des State IDs depuis les noms
        uint onboardingIntroID = AkUnitySoundEngine.GetIDFromString(onboardingIntro);
        uint onboardingCurtainID = AkUnitySoundEngine.GetIDFromString(onboardingCurtain);
        uint onboardingSpotsID = AkUnitySoundEngine.GetIDFromString(onboardingSpots);
        uint interruptionCurtainID = AkUnitySoundEngine.GetIDFromString(interruptionCurtain);
        uint interruptionSpotsID = AkUnitySoundEngine.GetIDFromString(interruptionSpots);
        uint roueStateID = AkUnitySoundEngine.GetIDFromString(roueState);
        uint combatStateID = AkUnitySoundEngine.GetIDFromString(combatState);


        // TODO ajouter le ton du narrateur
        //Rajouter des states ici si on en a besoin de + !

        //if (onboarding_introID == 0 || onboarding_curtainID == 0 || onboarding_spotID == 0)
        //{
        //    DebugLog($"Échec de conversion des states: {repetition}, {etat}, {objet}, {narra}", LogType.Error);
        //    return;
        //}

        uint[] args = new uint[]
        {
        onboardingIntroID,
        onboardingCurtainID,
        onboardingSpotsID,
        interruptionCurtainID,
        interruptionSpotsID,
        roueStateID,
        combatStateID
        };

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
            _ = AkUnitySoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
            _ = AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
            return;
        }

        _ = playlist.Enqueue(nodeID);
        _ = AkUnitySoundEngine.DynamicSequenceUnlockPlaylist(sequenceID);
        _ = AkUnitySoundEngine.DynamicSequencePlay(sequenceID);

        if (await UniTask.WaitUntil(() => isEnded, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
        {
            DebugLog($"PlayDialogueWithStatesAsync interrupted");
            _ = AkUnitySoundEngine.DynamicSequenceStop(sequenceID);
            _ = AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
            throw new OperationCanceledException();
        }

        _ = AkUnitySoundEngine.DynamicSequenceClose(sequenceID);
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
            case LogType.Assert:
                break;
            case LogType.Exception:
                break;
            default:
                break;
        }
    }
}
