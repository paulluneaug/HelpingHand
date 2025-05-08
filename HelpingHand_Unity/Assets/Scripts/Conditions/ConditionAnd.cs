using System.Linq;

using UnityEngine;

[System.Serializable]
public class ConditionAnd : ConditionBase
{
    [SerializeField]
    private ConditionBase[] m_preconditions = new ConditionBase[] {};

    public ConditionBase[] Preconditions => m_preconditions;

    public override bool Test()
    {
        return m_preconditions.All(p => p.Test());
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
}
