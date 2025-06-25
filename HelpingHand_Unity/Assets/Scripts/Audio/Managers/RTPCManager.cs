using AK.Wwise;

using Sirenix.OdinInspector;

using UnityEngine;

public class RTPCManager : MonoBehaviour
{
    #region RTPCs

    [TitleGroup("Music RTPCs")]
    [FoldoutGroup("Music RTPCs/Parameters")] public RTPC FirstMusic_FirstLayer;
    [FoldoutGroup("Music RTPCs/Parameters")] public RTPC FirstMusic_SecondLayer;

    [TitleGroup("Ambience RTPCs")]
    [FoldoutGroup("Ambience RTPCs/Parameters")] public RTPC AmbienceLevelRTPC;
    [FoldoutGroup("Ambience RTPCs/Parameters")] public RTPC AudienceLevelRTPC;

    [TitleGroup("Bus RTPCs")]
    public RTPC RTPC_MasterVolume;
    public RTPC RTPC_VoiceVolume;
    public RTPC RTPC_UIVolume;
    public RTPC RTPC_SFXVolume;
    public RTPC RTPC_MusicVolume;

    public RTPC RTPC_TimeOfDay;

    public BaseVariable<float> TimeOfDay;

    #endregion

    [Header("Default Target Object")]
    public GameObject TargetObject;

    #region Enums

    public enum MusicRtpcType
    {
        Layer1,
        Layer2
    }

    public enum AmbienceRtpcType
    {
        RumbleLevel,
        AudienceLevel
    }

    public enum BusRtpcType
    {
        Master,
        Voice,
        UI,
        Music,
        SFX
    }

    #endregion

    public void InitRtpcManager()
    {
        if (TargetObject == null)
        {
            TargetObject = gameObject;
        }
    }

    #region Set Methods

    public void SetMusicRtpc(MusicRtpcType type, float value)
    {
        switch (type)
        {
            case MusicRtpcType.Layer1:
                FirstMusic_FirstLayer.SetValue(TargetObject, value);
                break;
            case MusicRtpcType.Layer2:
                FirstMusic_SecondLayer.SetValue(TargetObject, value);
                break;
            default:
                Debug.LogWarning($"[RTPCManager] Unknown MusicRtpcType: {type}");
                break;
        }
    }

    public void SetAmbienceRtpc(AmbienceRtpcType type, float value)
    {
        switch (type)
        {
            case AmbienceRtpcType.RumbleLevel:
                AmbienceLevelRTPC.SetValue(TargetObject, value);
                break;
            case AmbienceRtpcType.AudienceLevel:
                AudienceLevelRTPC.SetValue(TargetObject, value);
                break;
            default:
                Debug.LogWarning($"[RTPCManager] Unknown AmbienceRtpcType: {type}");
                break;
        }
    }

    public void SetBusRtpc(BusRtpcType type, float value)
    {
        switch (type)
        {
            case BusRtpcType.Master:
                RTPC_MasterVolume.SetValue(TargetObject, value);
                break;
            case BusRtpcType.Voice:
                RTPC_VoiceVolume.SetValue(TargetObject, value);
                break;
            case BusRtpcType.UI:
                RTPC_UIVolume.SetValue(TargetObject, value);
                break;
            case BusRtpcType.Music:
                RTPC_MusicVolume.SetValue(TargetObject, value);
                break;
            case BusRtpcType.SFX:
                RTPC_SFXVolume.SetValue(TargetObject, value);
                break;
            default:
                Debug.LogWarning($"[RTPCManager] Unknown BusRtpcType: {type}");
                break;
        }
    }

    #endregion
}
