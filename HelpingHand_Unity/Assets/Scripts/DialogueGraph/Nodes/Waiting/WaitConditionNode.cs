using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(350)]
[CreateNodeMenu("Waiting/Wait Condition")]
[NodeTint(0.2f, 0.1f, .3f)]
public class WaitConditionNode : WaitNodeBase
{
    [Space]
    [SerializeField]
    [HideLabel]
    private ConditionBase m_condition = new ConditionAnd();


    public override void Initialize()
    {
        base.Initialize();
        m_condition.Initialize();
    }

    protected override void InitializeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        m_condition.OnPreconditionUpdated -= UpdateWaitUntilTest;
        m_condition.OnPreconditionUpdated += UpdateWaitUntilTest;
    }

    protected override void DisposeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        m_condition.OnPreconditionUpdated -= UpdateWaitUntilTest;
    }

    protected override void UpdateWaitUntilTest()
    {
        m_stopWait = m_condition.Test();
    }

    public override void Dispose()
    {
        base.Dispose();
        m_condition.Dispose();
    }
}