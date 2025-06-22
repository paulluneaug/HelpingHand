using System;

using UnityEngine;

public abstract class AudioEventController : IDisposable
{
    [SerializeField] private GameObject m_audioSource;

    [NonSerialized] protected GameObject m_source;
    public virtual void Init(GameObject defaultSource)
    {
        m_source = m_audioSource != null ? m_audioSource : defaultSource;
    }

    public abstract void Dispose();
}
