using System;

using Events;

using UnityEngine;

using WwiseEvent = AK.Wwise.Event;

public class GameEventAudioPlayer : MonoBehaviour
{
    [SerializeField] private BaseGameEvent m_gameEvent;
    [SerializeField] private WwiseEvent m_audioEvent;

    private void Awake()
    {
        m_gameEvent.AddListener(OnGameEventRaised);
    }

    private void OnDestroy()
    {
        m_gameEvent.RemoveListener(OnGameEventRaised);
    }

    private void OnGameEventRaised()
    {
        _ = m_audioEvent.Post(gameObject);
    }
}
