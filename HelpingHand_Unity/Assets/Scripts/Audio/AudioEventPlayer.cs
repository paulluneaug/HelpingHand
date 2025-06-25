using UnityEngine;

using WwiseEvent = AK.Wwise.Event;

public class AudioEventPlayer : MonoBehaviour
{
    [SerializeField] private WwiseEvent m_audioEvent;

    public void PostEvent()
    {
        _ = m_audioEvent.Post(gameObject);
    }
}
