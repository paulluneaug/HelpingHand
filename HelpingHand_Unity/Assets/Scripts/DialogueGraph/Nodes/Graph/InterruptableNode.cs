using Sirenix.OdinInspector;

using UnityEngine;

public abstract class InterruptableNode : KillableNode
{
    [Space]
    [SerializeField]
    [LabelWidth(100)]
    private bool m_interruptable = true;

    public bool Interruptable => m_interruptable;

    protected bool m_hasBeenInterrupted;

    public override void Initialize()
    {
        base.Initialize();
        m_hasBeenInterrupted = false;
    }
}