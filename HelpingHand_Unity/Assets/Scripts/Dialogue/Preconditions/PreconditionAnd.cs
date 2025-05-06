using System.Linq;

using UnityEngine;

[System.Serializable]
public class PreconditionAnd : PreconditionBase
{
    [SerializeField]
    private PreconditionBase[] m_preconditions = new PreconditionBase[] {};

    public PreconditionBase[] Preconditions => m_preconditions;

    public override bool Test()
    {
        return m_preconditions.All(p => p.Test());
    }

    public override void Initialize()
    {
        base.Initialize();
        foreach (PreconditionBase precondition in m_preconditions)
        {
            precondition.Initialize();
            precondition.OnPreconditionUpdated -= RaiseOnPreconditionUpdated;
            precondition.OnPreconditionUpdated += RaiseOnPreconditionUpdated;
        }
    }
}
