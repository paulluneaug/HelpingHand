using Sirenix.OdinInspector;

using UnityEngine;

using TitleAlignments = Sirenix.OdinInspector.TitleAlignments;
using WwiseEvent = AK.Wwise.Event;
public class EventManager : MonoBehaviour
{
    [InfoBox("➔<b>EventName.Post(gameObject);</b>", InfoMessageType.None)]
    [TitleGroup("Events", horizontalLine: true, alignment: TitleAlignments.Centered, boldTitle: true, indent: true)]
    public string Temp;
    #region Music Events
    [FoldoutGroup("Music Events")]
    [FoldoutGroup("Music Events")][LabelWidth(200)] public WwiseEvent MainMusic_Play;
    [FoldoutGroup("Music Events")][LabelWidth(200)] public WwiseEvent MainMusic_Stop;
    #endregion

    #region MovableObjects Events
    [FoldoutGroup("MovableObjects Events")]
    [FoldoutGroup("MovableObjects Events/SquareRock")][LabelWidth(200)] public WwiseEvent SquareRockLoop_Play;
    [FoldoutGroup("MovableObjects Events/SquareRock")][LabelWidth(200)] public WwiseEvent SquareRockMin_Play;
    [FoldoutGroup("MovableObjects Events/SquareRock")][LabelWidth(200)] public WwiseEvent SquareRockMax_Play;
    [FoldoutGroup("MovableObjects Events/SquareRock")][LabelWidth(200)] public WwiseEvent SquareRockFadeout_Play;
    [FoldoutGroup("MovableObjects Events/SquareRock")][LabelWidth(200)] public WwiseEvent SquareRockImmediate_Play;
    #endregion

    #region Controller Events
    [FoldoutGroup("Controller Events")]
    [FoldoutGroup("Controller Events/Light Events")][LabelWidth(200)] public WwiseEvent SpotlightOn_Play;
    [FoldoutGroup("Controller Events/Light Events")][LabelWidth(200)] public WwiseEvent SpotlightOff_Play;

    [FoldoutGroup("Controller Events/Music Events")][LabelWidth(200)] public WwiseEvent FightingJingle_Play;
    [FoldoutGroup("Controller Events/Music Events")][LabelWidth(200)] public WwiseEvent HeroicJingle_Play;
    [FoldoutGroup("Controller Events/Music Events")][LabelWidth(200)] public WwiseEvent HorrorJingle_Play;
    [FoldoutGroup("Controller Events/Music Events")][LabelWidth(200)] public WwiseEvent SadJingle_Play;
    [FoldoutGroup("Controller Events/Music Events")][LabelWidth(200)] public WwiseEvent Jingle_Stop;

    [FoldoutGroup("Controller Events/SFX Events")][LabelWidth(200)] public WwiseEvent Monster_Play;
    [FoldoutGroup("Controller Events/SFX Events")][LabelWidth(200)] public WwiseEvent Weapon_Play;
    #endregion
    #region UI Events
    [FoldoutGroup("UI Events")]
    [FoldoutGroup("UI Events/Fader Events")][LabelWidth(200)] public WwiseEvent FaderLoop_Play;
    [FoldoutGroup("UI Events/Fader Events")][LabelWidth(200)] public WwiseEvent FaderMin_Play;
    [FoldoutGroup("UI Events/Fader Events")][LabelWidth(200)] public WwiseEvent FaderMax_Play;
    [FoldoutGroup("UI Events/Fader Events")][LabelWidth(200)] public WwiseEvent FaderFadeout_Stop;
    [FoldoutGroup("UI Events/Fader Events")][LabelWidth(200)] public WwiseEvent FaderImmediate_Stop;

    [FoldoutGroup("UI Events/Button Events")][LabelWidth(200)] public WwiseEvent ButtonOnPointerUp_Play;
    [FoldoutGroup("UI Events/Button Events")][LabelWidth(200)] public WwiseEvent ButtonOnPointerDown_Play;
    [FoldoutGroup("UI Events/Button Events")][LabelWidth(200)] public WwiseEvent ButtonOnPointerEnter_Play;
    [FoldoutGroup("UI Events/Button Events")][LabelWidth(200)] public WwiseEvent ButtonOnPointerExit_Play;

    [FoldoutGroup("UI Events/Button Events")][LabelWidth(200)] public WwiseEvent Toggle_Play;
    [FoldoutGroup("UI Events/Button Events")][LabelWidth(200)] public WwiseEvent Untoggle_Play;

    [FoldoutGroup("UI Events/Menu Events")][LabelWidth(200)] public WwiseEvent MenuOpenSound_Play;
    [FoldoutGroup("UI Events/Menu Events")][LabelWidth(200)] public WwiseEvent MenuCloseSound_Play;
    [FoldoutGroup("UI Events/Menu Events")][LabelWidth(200)] public WwiseEvent Typewriter_Play;

    [FoldoutGroup("UI Events/Menu Events")][LabelWidth(200)] public WwiseEvent OnSelectButton_Play;
    [FoldoutGroup("UI Events/Menu Events")][LabelWidth(200)] public WwiseEvent OnClick_Play;
    [FoldoutGroup("UI Events/Menu Events")][LabelWidth(200)] public WwiseEvent GearTransitionLoop_Play;
    [FoldoutGroup("UI Events/Menu Events")][LabelWidth(200)] public WwiseEvent GearTransitionLoop_Stop;

    [FoldoutGroup("UI Events/Menu Events")][LabelWidth(200)] public WwiseEvent ArrowButtonClick_Play;
    [FoldoutGroup("UI Events/Menu Events")][LabelWidth(200)] public WwiseEvent OptionsTopButtonClick_Play;

    #endregion
    #region Puppet Events
    [FoldoutGroup("Puppet Events")][LabelWidth(200)] public WwiseEvent Footsteps_Play;
    [FoldoutGroup("Puppet Events")][LabelWidth(200)] public WwiseEvent Footsteps_Stop;
    #endregion

    #region Ambience Events
    [FoldoutGroup("Ambience Events")][LabelWidth(200)] public WwiseEvent RoomMachinist_Ambience_Play;
    [FoldoutGroup("Ambience Events")][LabelWidth(200)] public WwiseEvent Theater_Ambience_Play;
    #endregion

    #region Gears Events
    [FoldoutGroup("Ambience Events")][LabelWidth(200)] public WwiseEvent MainGear_Play;
    #endregion

    #region Music Events 
    [FoldoutGroup("Music Events")][LabelWidth(200)] public WwiseEvent SwitchToBattleMusic;
    [FoldoutGroup("Music Events")][LabelWidth(200)] public WwiseEvent SwitchToSadMusic;
    [FoldoutGroup("Music Events")][LabelWidth(200)] public WwiseEvent SwitchToSuspenseMusic;
    [FoldoutGroup("Music Events")][LabelWidth(200)] public WwiseEvent SwitchToHorribleMusic;

    #endregion

    #region ItemDropper Events 
    [FoldoutGroup("ItemDropper Events")][LabelWidth(200)] public WwiseEvent Play_FallingObject;

    [FoldoutGroup("ItemDropper Events")][LabelWidth(200)] public WwiseEvent Play_HatCatch;
    [FoldoutGroup("ItemDropper Events")][LabelWidth(200)] public WwiseEvent Play_WeaponCatch;

    [FoldoutGroup("ItemDropper Events")][LabelWidth(200)] public WwiseEvent Play_ItemDropperBox_Open;
    [FoldoutGroup("ItemDropper Events")][LabelWidth(200)] public WwiseEvent Play_ItemDropperMovingLoop;

    [FoldoutGroup("ItemDropper Events")][LabelWidth(200)] public WwiseEvent Stop_ItemDropperMovingLoop_Fadeout;
    [FoldoutGroup("ItemDropper Events")][LabelWidth(200)] public WwiseEvent Play_Smoke_Impact;


    [FoldoutGroup("ItemDropper Events")][LabelWidth(200)] public WwiseEvent Play_ItemDropperLeaving;

    [FoldoutGroup("ItemDropper Events")][LabelWidth(200)] public WwiseEvent Play_ItemDropperError;

    //Hat
    [FoldoutGroup("ItemDropper Events/Hat")][LabelWidth(200)] public WwiseEvent SelectedBeret;
    [FoldoutGroup("ItemDropper Events/Hat")][LabelWidth(200)] public WwiseEvent SelectedChicken;
    [FoldoutGroup("ItemDropper Events/Hat")][LabelWidth(200)] public WwiseEvent SelectedHatEmpty;
    [FoldoutGroup("ItemDropper Events/Hat")][LabelWidth(200)] public WwiseEvent SelectedHeadphones;
    [FoldoutGroup("ItemDropper Events/Hat")][LabelWidth(200)] public WwiseEvent SelectedHelmet;
    [FoldoutGroup("ItemDropper Events/Hat")][LabelWidth(200)] public WwiseEvent SelectedRabbit;

    //Weapon
    [FoldoutGroup("ItemDropper Events/Weapon")][LabelWidth(200)] public WwiseEvent SelectedBaguette;
    [FoldoutGroup("ItemDropper Events/Weapon")][LabelWidth(200)] public WwiseEvent SelectedBeer;
    [FoldoutGroup("ItemDropper Events/Weapon")][LabelWidth(200)] public WwiseEvent SelectedBroom;
    [FoldoutGroup("ItemDropper Events/Weapon")][LabelWidth(200)] public WwiseEvent SelectedCarot;
    [FoldoutGroup("ItemDropper Events/Weapon")][LabelWidth(200)] public WwiseEvent SelectedEmpty;
    [FoldoutGroup("ItemDropper Events/Weapon")][LabelWidth(200)] public WwiseEvent SelectedSword;



    #endregion 


    [System.Obsolete]
    public uint PostWithCallback(GameObject gameObject, WwiseEvent wwiseEvent, AkCallbackType callbackType, AkCallbackManager.EventCallback callback, object cookie = null)
    {
        // Utilise l'ID de l'événement directement à partir du WwiseEvent
        uint eventID = (uint)wwiseEvent.ID;

        // Joue l'événement avec le callback
        return AkUnitySoundEngine.PostEvent(eventID, gameObject, (uint)callbackType, callback, cookie);
    }

}
