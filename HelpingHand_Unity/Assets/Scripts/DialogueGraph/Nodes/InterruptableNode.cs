using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

public abstract class InterruptableNode : BaseNode
{
    [SerializeField][LabelWidth(100)]
    private bool m_interruptable = false;

    public bool Interruptable => m_interruptable;
    
    protected bool m_hasBeenInterrupted;

    public override void Initialize()
    {
        m_hasBeenInterrupted = false;
    }
}