using System;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(350)]
public class InterruptWithConditionNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;
    
    [SerializeField]
    private ConditionBase m_condition;

    [Output]
    public DialogueFlow m_out;

    [Space] [SerializeField]
    private bool m_doesTimeout;

    [Output] [ShowIf(nameof(m_doesTimeout))]
    public DialogueFlow m_timeoutOut;

    [SerializeField] [ShowIfGroup(nameof(m_doesTimeout))]
    private float m_timeout;

    private TimeoutController m_timeoutController;
    private bool m_isConditionPassed;

    protected override void Init()
    {
        base.Init();
        m_description = "Wait for the condition to be true & the current dialogue to be interruptable";
    }

    public override void Initialize()
    {
        m_timeoutController = new TimeoutController();
        m_isConditionPassed = false;
        m_condition.Initialize();
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
    }

    public override object GetValue(NodePort port)
    {
        return port.fieldName switch
        {
            nameof(m_out) => m_out,
            nameof(m_in) => m_in,
            nameof(m_timeoutOut) => m_timeoutOut,
            _ => null
        };
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        DebugLog($"Waiting for condition");
        
        OnConditionUpdated();
        UniTask task = UniTask.WaitUntil(TestInterruption, PlayerLoopTiming.Update, handler.StopToken);
        if (m_doesTimeout)
        {
            DebugLog($"With timeout ({m_timeout} seconds)");
            m_timeoutController.Reset();
            task = task.AttachExternalCancellation(m_timeoutController.Timeout(TimeSpan.FromSeconds(m_timeout)));
        }
        
        if (await task.SuppressCancellationThrow())
        {
            DebugLog($"Wait interrupted");

            if (m_timeoutController.IsTimeout())
            {
                DebugLog($"Wait timeout");
            }

            return;
        }

        DebugLog($"Interrupting main graph");

        GraphManager.Instance.Interrupt(handler);

        await UniTask.NextFrame();
        await ContinueFlow(handler);
    }

    private bool TestInterruption()
    {
        return GraphManager.Instance.CurrentNodeCanBeInterrupted && m_isConditionPassed;
    }

    private void OnConditionUpdated()
    {
        m_isConditionPassed = m_condition.Test();
    }

    private void OnDestroy()
    {
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
    }
}