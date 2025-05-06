using System.Linq;

using UnityEngine;

[System.Serializable]
public class PreconditionOr : PreconditionBase
{
    [SerializeField]
    private PreconditionBase[] m_preconditions;
    
    public PreconditionBase[] Preconditions => m_preconditions;

    public override bool Test()
    {
        return m_preconditions.Any(p => p.Test());
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
