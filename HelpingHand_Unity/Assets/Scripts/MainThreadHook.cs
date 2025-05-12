using System;

using UnityUtility.Singletons;

public class MainThreadHook : MonoBehaviourSingleton<MainThreadHook>
{
    private readonly object m_lock = new();
    private Action m_callback;

    private void Update()
    {
        Action action = null;
        lock (m_lock)
        {
            action = m_callback;
            m_callback = null;
        }
        action?.Invoke();
    }

    // Can be called from another thread
    public void Invoke(Action a)
    {
        m_callback += a;
    }
}