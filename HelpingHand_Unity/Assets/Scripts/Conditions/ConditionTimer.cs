using UnityEngine;

[System.Serializable]
public class ConditionTimer : ConditionBase
{
    [SerializeField]
    private StandaloneTimer m_timer;

    public override void Initialize()
    {
        base.Initialize();
        m_timer.Initialize();
        m_timer.RemoveListener(RaiseOnPreconditionUpdated);
        m_timer.AddListener(RaiseOnPreconditionUpdated);
    }

    public override void Dispose()
    {
        base.Dispose();
        m_timer.RemoveListener(RaiseOnPreconditionUpdated);
    }

    public override bool Test()
    {
        return m_timer.Elapsed;
    }
}
