using System.Collections.Generic;

using UnityEngine;

using UnityUtility.Singletons;
using UnityUtility.Timer;

public class StandaloneTimerSingleton : MonoBehaviourSingleton<StandaloneTimerSingleton>
{
    private List<Timer> m_timers;

    public override void Initialize()
    {
        base.Initialize();
        m_timers = new();
    }

    private void Update()
    {
        for (int i = m_timers.Count - 1; i >= 0; i--)
        {
            Timer timer = m_timers[i];
            timer.Update(Time.deltaTime);
        }
    }

    public void PushTimer(Timer timer)
    {
        m_timers.Add(timer);
    }

    public void RemoveTimer(Timer timer)
    {
        m_timers.Remove(timer);
    }
}
