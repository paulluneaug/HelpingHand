using UnityEngine;

[System.Serializable]
public class PreconditionTrigger : PreconditionBase
{
    [SerializeField]
    private InputTrigger m_trigger;

    public override bool Test()
    {
        return m_trigger.IsRaised;
    }

    public override void Initialize()
    {
        base.Initialize();
        m_trigger.Initialize();
        m_trigger.RaiseTriggerEvent -= RaiseOnPreconditionUpdated;
        m_trigger.RaiseTriggerEvent += RaiseOnPreconditionUpdated;
    }
}
