using System;

using UnityEngine;

using WwiseEvent = AK.Wwise.Event;

[Serializable]
public class WwiseInputEventPair
{
    [SerializeField] private WwiseEvent m_activeEvent;
    [SerializeField] private WwiseEvent m_inactiveEvent;

    public uint PostEvent(bool active, GameObject gameObject)
    {
        WwiseEvent eventToPost = active ? m_activeEvent : m_inactiveEvent;
        uint? eventID = eventToPost?.Post(gameObject);
        return eventID.HasValue ? eventID.Value : AkUnitySoundEngine.AK_INVALID_PLAYING_ID;
    }
}
