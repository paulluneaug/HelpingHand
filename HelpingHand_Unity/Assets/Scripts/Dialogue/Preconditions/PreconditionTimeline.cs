using UnityUtility.Extensions;

[System.Serializable]
public class PreconditionTimeline : PreconditionBase
{
    public double m_start;
    public double m_end;
    public bool m_stayActive;
    
    public override bool Test()
    {
        return m_stayActive ? TimelineManager.Instance.Time > m_start : TimelineManager.Instance.Time.Between(m_start, m_end);
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
