using Events;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(250)]
[CreateNodeMenu("Waiting/Wait Event")]
[NodeTint(0.2f, 0.1f, .3f)]
public class WaitEventNode : WaitNodeBase
{
    [Space]
    [SerializeField]
    [HideLabel]
    private BaseGameEvent m_event;

    protected override void InitializeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        m_event.RemoveListener(UpdateWaitUntilTest);
        m_event.AddListener(UpdateWaitUntilTest);
    }

    protected override void DisposeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        m_event.RemoveListener(UpdateWaitUntilTest);
    }

    protected override void UpdateWaitUntilTest()
    {
        m_stopWait = true;
    }

    public override void Dispose()
    {
        base.Dispose();
        m_event.RemoveListener(UpdateWaitUntilTest);
    }
}