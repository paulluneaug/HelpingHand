using System;

using Sirenix.OdinInspector;

using UnityEngine;

[System.Serializable]
public abstract class ConditionBase : IDisposable
{
    public event Action OnPreconditionUpdated;

    [SerializeField]
    [LabelWidth(100)]
    [PropertySpace(0, 8)]
    protected int m_scoreMult = 1;

    protected void RaiseOnPreconditionUpdated()
    {
        OnPreconditionUpdated?.Invoke();
    }

    public abstract bool Test();

    public virtual void Initialize()
    {
        OnPreconditionUpdated = null;
    }

    public virtual void Dispose()
    {
        OnPreconditionUpdated = null;
    }

    public virtual int Score()
    {
        return 1 * m_scoreMult;
    }
}
