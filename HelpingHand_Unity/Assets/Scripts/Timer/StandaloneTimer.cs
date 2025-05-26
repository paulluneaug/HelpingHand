using System;

using Events;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Timer;

[Serializable]
[CreateAssetMenu(menuName = "Scriptable Objects/Timer")]
public class StandaloneTimer : BaseGameEvent
{
    [SerializeField]
    private bool m_isRepeating = false;

    [SerializeField]
    [ShowIf(nameof(m_isRepeating))]
    private float m_repeatInterval;

    [SerializeField]
    [HideIf(nameof(m_isRepeating))]
    private float m_duration;

    public bool Elapsed { get; private set; }

    private Timer m_timer;

    [Button("Initialize"), EnableIf("@UnityEngine.Application.isPlaying")]
    public void Initialize()
    {
        m_timer = new Timer(m_isRepeating ? m_repeatInterval : m_duration, m_isRepeating, 0);
        m_timer.OnTimerEnds += OnTimerEnded;
        StandaloneTimerSingleton.Instance.PushTimer(m_timer);
    }

    private void OnTimerEnded()
    {
        Elapsed = true;
        Raise();
        if (m_isRepeating)
        {
            Elapsed = false;
        }
        else
        {
            StandaloneTimerSingleton.Instance.RemoveTimer(m_timer);
        }
    }

    [Button("Start"), HorizontalGroup, EnableIf("@UnityEngine.Application.isPlaying")]
    public void Start()
    {
        m_timer.Start();
    }

    [Button("Pause"), HorizontalGroup, EnableIf("@UnityEngine.Application.isPlaying")]
    public void Pause()
    {
        m_timer.Pause(true);
    }

    [Button("Resume"), HorizontalGroup, EnableIf("@UnityEngine.Application.isPlaying")]
    public void Resume()
    {
        m_timer.Pause(false);
    }

    [Button("Stop"), HorizontalGroup, EnableIf("@UnityEngine.Application.isPlaying")]
    public void Stop()
    {
        m_timer.Stop();
    }

    private void OnDisable()
    {
        StandaloneTimerSingleton.Instance.RemoveTimer(m_timer);
        m_timer = null;
    }
}