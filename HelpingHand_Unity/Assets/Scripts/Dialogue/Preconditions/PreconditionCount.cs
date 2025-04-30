using UnityEngine;

[System.Serializable]
public class PreconditionCount : PreconditionBase
{
    [SerializeField]
    private PreconditionBase m_precondition;

    [SerializeField]
    private int m_countNeeded = 0;

    [SerializeField]
    private bool m_strictlyEquals = false;

    private int m_count;

    public override bool Test()
    {
        bool test = m_precondition.Test();
        if (test)
        {
            m_count++;
        }

        return test && (m_strictlyEquals ? m_count == m_countNeeded : m_count >= m_countNeeded);
    }

    public override void Initialize()
    {
        base.Initialize();
        m_count = 0;
        m_precondition.Initialize();
        m_precondition.OnPreconditionUpdated -= RaiseOnPreconditionUpdated;
        m_precondition.OnPreconditionUpdated += RaiseOnPreconditionUpdated;
    }
}