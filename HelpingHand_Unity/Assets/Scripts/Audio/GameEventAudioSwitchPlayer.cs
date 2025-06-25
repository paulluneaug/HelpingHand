using Events;

using UnityEngine;

using WwiseEvent = AK.Wwise.Event;

public class GameEventAudioSwitchPlayer : MonoBehaviour
{
    [SerializeField] private BaseGameEvent m_gameEvent;
    [SerializeField] private WwiseEvent m_audioEventOn;
    [SerializeField] private WwiseEvent m_audioEventOff;

    private bool isOn = false;

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
        if (isOn)
        {
            _ = m_audioEventOff.Post(gameObject);
        }
        else
        {
            _ = m_audioEventOn.Post(gameObject);
        }

        isOn = !isOn; // Inverse l'état pour la prochaine fois
    }
}
