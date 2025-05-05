using UnityEngine;

[System.Serializable]
public class PreconditionTimer : PreconditionBase
{
    [SerializeField]
    private StandaloneTimer m_timer;

    public override bool Test()
    {
        return m_timer.Elapsed;
    }

    public override void Initialize()
    {
        base.Initialize();
        m_timer.Initialize();
        m_timer.RemoveListener(RaiseOnPreconditionUpdated);
        m_timer.AddListener(RaiseOnPreconditionUpdated);
    }
}
