using System;
using System.Collections.Generic;

using UnityEngine;

using UnityUtility.Singletons;
using UnityUtility.Timer;

public class StandaloneTimerSingleton : MonoBehaviourSingleton<StandaloneTimerSingleton>
{
    public event Action OnUpdateTickEvent;

    private List<Timer> m_timers;

    public override void Initialize()
    {
        base.Initialize();
        m_timers = new();
    }

    private void Update()
    {
        OnUpdateTickEvent?.Invoke();

        for (int i = m_timers.Count - 1; i >= 0; i--)
        {
            Timer timer = m_timers[i];
            _ = timer.Update(Time.deltaTime);
        }
    }

    public void PushTimer(Timer timer)
    {
        m_timers.Add(timer);
    }

    public void RemoveTimer(Timer timer)
    {
        _ = m_timers.Remove(timer);
    }
}
