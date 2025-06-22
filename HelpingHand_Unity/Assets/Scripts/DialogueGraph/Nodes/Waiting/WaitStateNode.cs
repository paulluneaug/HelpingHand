using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Waiting/Wait State")] 
[NodeTint(0.2f, 0.1f, .3f)] 
[NodeWidth(300)]
public class WaitStateNode : WaitNodeBase
{
    [Space]
    [SerializeField]
    [HideLabel]
    private EntityState m_state;

    [SerializeField]
    [LabelText("Must be set?")]
    private bool m_mustBeSet = true;

    protected override void InitializeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        m_state.RemoveListener(UpdateWaitUntilTest);
        m_state.AddListener(UpdateWaitUntilTest);
    }

    protected override void DisposeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        m_state.RemoveListener(UpdateWaitUntilTest);
    }

    protected override void UpdateWaitUntilTest()
    {
        m_stopWait = m_state.IsSet == m_mustBeSet;
    }
}