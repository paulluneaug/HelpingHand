using UnityEngine;

[System.Serializable]
public class PreconditionEntityState : PreconditionBase
{
    [SerializeField]
    private EntityState m_state;

    public override bool Test()
    {
        return m_state.IsSet;
    }

    public override void Initialize()
    {
        base.Initialize();
        m_state.RemoveListener(RaiseOnPreconditionUpdated);
        m_state.AddListener(RaiseOnPreconditionUpdated);
    }
}
