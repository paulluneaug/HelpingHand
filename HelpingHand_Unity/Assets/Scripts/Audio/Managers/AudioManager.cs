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
using UnityEngine.SceneManagement;
using System.Text;

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

    public UnityEngine.SceneManagement.Scene MainMenu;
    public UnityEngine.SceneManagement.Scene Gameplay;

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

    #region Scene change
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "EntryScene":
                StateManager.SetGameState(GameState.MainMenu);
                StateManager.SetMusicState(MusicState.MainMenu);
                _ = EventManager.MainMusic_Play.Post(gameObject);
                DebugLog("EntryScene - MainMenu music playing");
                break;

            case "0_Onboarding":
                StateManager.SetGameState(GameState.Gameplay);
                StateManager.SetMusicState(MusicState.Onboarding_1);
                Debug.Log("Switched music to Onboarding 1st theme");
                break;

            default:
               // StateManager.SetGameState(GameState.None);
               // StateManager.SetMusicState(MusicState.None);
                break;
        }
    }
    #endregion

    private void Awake()
    {
        SoundbankManager.LoadAllSoundbanks(); //Charge toutes les soundbanks: temporaire ! Load les soundbanks au fur et à mesure pour optimiser
    }

    protected override void Start()
    {
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
    // Méthode qui joue un Dialogue Event avec série de string qui représentent le nom des States, gameObject sur lequel les sons sont joués, token d'annulation
    public async UniTask PlayDialogueWithStatesAsync(
        string onboardingIntro,
        string onboardingCurtain,
        string onboardingSpots,
        string interruptionCurtain,
        string interruptionSpots,
        string roueState,
        string combatState,
        string interruptionRoue,
        string interruptionSucces,
        string equipementState,
        string finState,
        string successState,
        GameObject targetObject = null,
        CancellationToken cancellationToken = default)
    {
        //DebugLog($"PlayDialogueWithStatesAsync: {onboardingIntro}, {onboardingCurtain}, {onboardingSpots}, {interruptionCurtain}, {interruptionSpots}, {roueState}, {combatState}");

        //Récupération de l’ID Wwise de l’event Dialogue_Event -> sensible à la casse
        uint dialogueEventId = AkUnitySoundEngine.GetIDFromString("Dialogue_Event");

        if (dialogueEventId == AkUnitySoundEngine.AK_INVALID_UNIQUE_ID)
        {
            DebugLog($"L'event 'Dialogue_Event' est introuvable. Vérifier son nom et sa SoundBank.", LogType.Error);
            return;
        }

        // uint = ID interne Wwise associé à un string (pas de casse et de lien direct avec Wwise puisque dans ce cas on associe les IDs aux states dans le tableau juste après
        uint onboardingIntroID = AkUnitySoundEngine.GetIDFromString(onboardingIntro);
        uint onboardingCurtainID = AkUnitySoundEngine.GetIDFromString(onboardingCurtain);
        uint onboardingSpotsID = AkUnitySoundEngine.GetIDFromString(onboardingSpots);

        uint interruptionCurtainID = AkUnitySoundEngine.GetIDFromString(interruptionCurtain);
        uint interruptionSpotsID = AkUnitySoundEngine.GetIDFromString(interruptionSpots);
        uint interruptionRoueID = AkUnitySoundEngine.GetIDFromString(interruptionRoue);
        uint interruptionSuccesID = AkUnitySoundEngine.GetIDFromString(interruptionSucces);

        uint equipementID = AkUnitySoundEngine.GetIDFromString(equipementState);
        uint roueStateID = AkUnitySoundEngine.GetIDFromString(roueState);
        uint combatStateID = AkUnitySoundEngine.GetIDFromString(combatState);
        uint finStateID = AkUnitySoundEngine.GetIDFromString(finState);

        uint SuccesID = AkUnitySoundEngine.GetIDFromString(successState);

        //Tableau d'argument  qui contient la liste des ID qui correspondent à chaque valeur (State) de monDialogue Event -> même ordre que dans Wwise!
        uint[] args = new uint[]
        {
        onboardingIntroID,
        onboardingCurtainID,
        onboardingSpotsID,

        interruptionCurtainID,
        interruptionSpotsID,
        interruptionRoueID,
        interruptionSuccesID,

        equipementID,
        roueStateID,
        combatStateID,
        finStateID,

        SuccesID,
        };

        #region Debug
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Dialogue State Argument Wwise :");

        void AppendIfNotIgnore(string label, string value, uint id)
        {
            if (!string.Equals(value, "IGNORE", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append($" {label}: {value}");
            }
        }

        AppendIfNotIgnore("Onboarding Intro", onboardingIntro, onboardingIntroID);
        AppendIfNotIgnore("Onboarding Curtain", onboardingCurtain, onboardingCurtainID);
        AppendIfNotIgnore("Onboarding Spots", onboardingSpots, onboardingSpotsID);

        AppendIfNotIgnore("Interruption Curtain", interruptionCurtain, interruptionCurtainID);
        AppendIfNotIgnore("Interruption Spots", interruptionSpots, interruptionSpotsID);
        AppendIfNotIgnore("Interruption Roue", interruptionRoue, interruptionRoueID);
        AppendIfNotIgnore("Interruption Succès", interruptionSucces, interruptionSuccesID);

        AppendIfNotIgnore("Équipement", equipementState, equipementID);
        AppendIfNotIgnore("Roue", roueState, roueStateID);
        AppendIfNotIgnore("Combat", combatState, combatStateID);
        AppendIfNotIgnore("Fin", finState, finStateID);
        AppendIfNotIgnore("Succès", successState, SuccesID);

        Debug.Log(sb.ToString());


        #endregion

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
