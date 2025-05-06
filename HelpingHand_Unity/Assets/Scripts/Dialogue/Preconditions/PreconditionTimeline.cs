using UnityUtility.Extensions;

[System.Serializable]
public class PreconditionTimeline : PreconditionBase
{
    public double m_start;
    public double m_end;
    public bool m_stayActive;

    public override void Initialize()
    {
        base.Initialize();
    }
    
    public override bool Test()
    {
        return m_stayActive ? TimelineManager.Instance.CurrentRunner.Time > m_start : TimelineManager.Instance.CurrentRunner.Time.Between(m_start, m_end);
    }
}
