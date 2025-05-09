using System;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

public class WaitStateNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [SerializeField] [HideLabel]
    private EntityState m_state;

    [Output]
    public DialogueFlow m_out;

    [Space] [SerializeField]
    private bool m_doesTimeout;

    [Output] [ShowIf(nameof(m_doesTimeout))]
    public DialogueFlow m_timeoutOut;

    [SerializeField] [ShowIfGroup(nameof(m_doesTimeout))]
    private float m_timeout;

    private TimeoutController m_timeoutController;

    public override void Initialize()
    {
        m_timeoutController = new TimeoutController();
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
        DebugLog($"Waiting for state {m_state.name}");

        UniTask task = UniTask.WaitUntil(() => m_state.IsSet, PlayerLoopTiming.Update, handler.StopToken);
        if (m_doesTimeout)
        {
            DebugLog($"With timeout ({m_timeout} seconds)");
            m_timeoutController.Reset();
            task = task.AttachExternalCancellation(m_timeoutController.Timeout(TimeSpan.FromSeconds(m_timeout)));
        }

        if (await task.SuppressCancellationThrow())
        {
            DebugLog($"Wait interrupted");

            if (!m_timeoutController.IsTimeout())
            {
                // The graph is being paused => We have to wait its reactivation
                await Execute(handler);
            }
        }

        if (m_timeoutController.IsTimeout())
        {
            DebugLog($"Wait timeout");
            await ContinueFlow(handler, GetOutputPort(nameof(m_timeoutOut)));
        }
        else
        {
            DebugLog($"State {m_state.name} is set, continue");
            await ContinueFlow(handler);
        }
    }
}