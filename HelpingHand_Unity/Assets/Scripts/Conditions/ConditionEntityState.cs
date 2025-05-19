using UnityEngine;

[System.Serializable]
public class ConditionEntityState : ConditionBase
{
    [SerializeField]
    private EntityState m_state;

    [SerializeField]
    private bool m_mustBeSet = true;

    public override void Initialize()
    {
        base.Initialize();
        m_state.RemoveListener(RaiseOnPreconditionUpdated);
        m_state.AddListener(RaiseOnPreconditionUpdated);
    }

    public override void Dispose()
    {
        base.Dispose();
        m_state.RemoveListener(RaiseOnPreconditionUpdated);
    }

    public override bool Test()
    {
        return m_mustBeSet == m_state.IsSet;
    }
}
