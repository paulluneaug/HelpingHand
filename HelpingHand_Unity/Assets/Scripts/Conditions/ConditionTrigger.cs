using UnityEngine;

[System.Serializable]
public class ConditionTrigger : ConditionBase
{
    [SerializeField]
    private InputTrigger m_trigger;

    public override void Initialize()
    {
        base.Initialize();
        m_trigger.Initialize();
        m_trigger.OnTriggered -= RaiseOnPreconditionUpdated;
        m_trigger.OnTriggered += RaiseOnPreconditionUpdated;
    }

    public override void Dispose()
    {
        base.Dispose();
        m_trigger.OnTriggered -= RaiseOnPreconditionUpdated;
    }

    public override bool Test()
    {
        return m_trigger.IsRaised;
    }
}
