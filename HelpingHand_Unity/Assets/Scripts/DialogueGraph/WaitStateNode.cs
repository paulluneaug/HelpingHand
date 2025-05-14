using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

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

    private CancellationTokenSource m_timeoutSource;
    
    public override void Initialize()
    {
    }
    
    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort port)
    {
        DebugLog($"Waiting for state {m_state.name}");

        CancellationToken cancellationToken = handler.StopToken;

        if (m_doesTimeout)
        {
            DebugLog($"With timeout ({m_timeout} seconds)");
            
            m_timeoutSource?.Dispose();
            m_timeoutSource = new ();
            m_timeoutSource.CancelAfterSlim(TimeSpan.FromSeconds(m_timeout));
            CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(handler.StopToken, m_timeoutSource.Token);
            cancellationToken = linkedTokenSource.Token;
        }
        
        UniTask task = UniTask.WaitUntil(() => m_state.IsSet, PlayerLoopTiming.Update, cancellationToken);

        if (await task.SuppressCancellationThrow())
        {
            DebugLog($"Wait interrupted");

            if (!m_timeoutSource.IsCancellationRequested)
            {
                DebugLog($"Paused/stopped");
                // The graph is being paused => We have to wait its reactivation
                await Execute(handler);
            }
        }

        if (m_timeoutSource is { IsCancellationRequested: true })
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