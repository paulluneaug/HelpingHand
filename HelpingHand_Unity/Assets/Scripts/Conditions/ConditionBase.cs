using System;

[System.Serializable]
public abstract class ConditionBase
{
    public event Action OnPreconditionUpdated;
    
    protected void RaiseOnPreconditionUpdated()
    {
        OnPreconditionUpdated?.Invoke();
    }
    
    public abstract bool Test();

    public virtual void Initialize()
    {
        OnPreconditionUpdated = null;
    }
}
