using System.Linq;

using UnityEngine;

[System.Serializable]
public class ConditionOr : ConditionBase
{
    [SerializeField]
    private ConditionBase[] m_preconditions;

    public ConditionBase[] Preconditions => m_preconditions;

    public override bool Test()
    {
        return m_preconditions.Any(p => p.Test());
    }

    public override void Initialize()
    {
        base.Initialize();
        foreach (ConditionBase precondition in m_preconditions)
        {
            precondition.Initialize();
            precondition.OnPreconditionUpdated -= RaiseOnPreconditionUpdated;
            precondition.OnPreconditionUpdated += RaiseOnPreconditionUpdated;
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        foreach (ConditionBase precondition in m_preconditions)
        {
            precondition.OnPreconditionUpdated -= RaiseOnPreconditionUpdated;
        }
    }

    public override int Depth()
    {
        int result = 0;
        foreach (ConditionBase precondition in m_preconditions)
        {
            result += precondition.Depth();
        }

        return result;
    }
}
