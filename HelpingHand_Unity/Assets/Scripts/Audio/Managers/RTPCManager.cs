using System.Collections.Generic;

using AK.Wwise;

using Sirenix.OdinInspector;

using UnityEngine;

public class RTPCManager : MonoBehaviour
{
    #region RTPCs
    [TitleGroup("Music RTPCs", Order = 0)]
    [InfoBox("➔ <b>SetMusicRtpc(MusicRtpcType.Layer1, 1f, gameObject);</b>", InfoMessageType.None)]
    [FoldoutGroup("Music RTPCs/Parameters")][LabelWidth(150)] public RTPC FirstMusic_FirstLayer;
    [FoldoutGroup("Music RTPCs/Parameters")][LabelWidth(150)] public RTPC FirstMusic_SecondLayer;

    [TitleGroup("Ambience RTPCs", Order = 0)]
    [InfoBox("➔ <b>SetAmbienceRtpc(AmbienceRtpcType.AudienceLevel, 0.8f, gameObject);</b>", InfoMessageType.None)]
    [FoldoutGroup("Ambience RTPCs/Parameters")][LabelWidth(150)] public RTPC AmbienceLevelRTPC;
    [FoldoutGroup("Ambience RTPCs/Parameters")][LabelWidth(150)] public RTPC AudienceLevelRTPC;
    #endregion

    #region Dictionnary
    private Dictionary<MusicRtpcType, RTPC> m_musicRtpcs;
    private Dictionary<AmbienceRtpcType, RTPC> m_ambienceRtpcs;
    #endregion

    [Header("Default Target Object")]
    [Tooltip("If no target is specified when setting an RTPC, it will default to this GameObject.")]
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
    #endregion
    public void InitRtpcDictionaries()
    {
        if (TargetObject == null)
        {
            TargetObject = gameObject;
        }

        m_musicRtpcs = new Dictionary<MusicRtpcType, RTPC>
        {
            { MusicRtpcType.Layer1, FirstMusic_FirstLayer },
            { MusicRtpcType.Layer2, FirstMusic_SecondLayer },
        };

        m_ambienceRtpcs = new Dictionary<AmbienceRtpcType, RTPC>
        {
            { AmbienceRtpcType.RumbleLevel, AmbienceLevelRTPC },
            { AmbienceRtpcType.AudienceLevel, AudienceLevelRTPC }
        };
    }
    public void SetMusicRtpc(MusicRtpcType type, float value, GameObject target = null)
    {
        if (m_musicRtpcs.TryGetValue(type, out var rtpcValue))
        {
            rtpcValue.SetValue(target ?? TargetObject, value);
        }
        else
        {
            Debug.LogWarning($"[WwiseRTPCManager] Music RTPC not found for: {type}");
        }
    }
    public void SetAmbienceRtpc(AmbienceRtpcType type, float value, GameObject target = null)
    {
        if (m_ambienceRtpcs.TryGetValue(type, out var rtpcValue))
        {
            rtpcValue.SetValue(target ?? TargetObject, value);
        }
        else
        {
            Debug.LogWarning($"[WwiseRTPCManager] Ambience RTPC not found for: {type}");
        }
    }
}
