using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

public class WaitStateNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField] [HideLabel]
    private EntityState m_state;

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

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        DebugLog($"Waiting for state {m_state.name}");

        CancellationToken cancellationToken = handler.StopToken;

        if (m_doesTimeout)
        {
            DebugLog($"With timeout ({m_timeout} seconds)");
            m_timeoutController.Reset();
            CancellationToken timeoutToken = m_timeoutController.Timeout(TimeSpan.FromSeconds(m_timeout));
            cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(handler.StopToken, timeoutToken).Token;
        }
        
        UniTask task = UniTask.WaitUntil(() => m_state.IsSet, PlayerLoopTiming.Update, cancellationToken);

        if (await task.SuppressCancellationThrow())
        {
            DebugLog($"Wait interrupted");

            if (!m_timeoutController.IsTimeout())
            {
                DebugLog($"Paused/stopped");
                // The graph is being paused => We have to wait its reactivation
                await Execute(handler);
            }
        }

        if (m_timeoutController.IsTimeout())
        {
            DebugLog($"Wait timeout");
            handler.ResetTimeout();
            await ContinueFlow(handler, GetOutputPort(nameof(m_timeoutOut)));
        }
        else
        {
            DebugLog($"State {m_state.name} is set, continue");
            await ContinueFlow(handler);
        }
    }
}