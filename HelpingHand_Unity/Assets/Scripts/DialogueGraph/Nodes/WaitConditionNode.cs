using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(350)] [CreateNodeMenu("Waiting/Wait Condition")] [NodeTint(0.2f, 0.1f, .3f)]
public class WaitConditionNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField] [HideLabel]
    private ConditionBase m_condition = new ConditionAnd();

    [Space] [SerializeField]
    private bool m_doesTimeout;

    [Output] [ShowIf(nameof(m_doesTimeout))]
    public DialogueFlow m_timeoutOut;

    [SerializeField] [ShowIfGroup(nameof(m_doesTimeout))]
    private float m_timeout;

    private CancellationTokenSource m_timeoutSource;
    private bool m_isConditionPassed;
    
    public override void Initialize()
    {
        m_isConditionPassed = false;
        m_condition.Initialize();
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
        
        while (true)
        {
            DebugLog($"Waiting for condition");

            OnConditionUpdated();
            
            if (!m_isConditionPassed)
            {
                CancellationToken cancellationToken = handler.StopToken;

                if (m_doesTimeout)
                {
                    DebugLog($"With timeout ({m_timeout} seconds)");
                    m_timeoutSource?.Dispose();
                    m_timeoutSource = new();
                    m_timeoutSource.CancelAfterSlim(TimeSpan.FromSeconds(m_timeout));
                    CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(handler.StopToken, m_timeoutSource.Token);
                    cancellationToken = linkedTokenSource.Token;
                }

                UniTask task = UniTask.WaitUntil(() => m_isConditionPassed, PlayerLoopTiming.Update, cancellationToken);

                if (await task.SuppressCancellationThrow())
                {
                    DebugLog($"Wait for condition has been interrupted");

                    if (!m_timeoutSource.IsCancellationRequested)
                    {
                        DebugLog($"Pause/stop requested");
                        // The graph is being paused => We have to wait its reactivation
                        await HandlePauseStop(handler);
                        continue;
                    }
                }
            }

            break;
        }
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
        
        if (m_timeoutSource is { IsCancellationRequested: true })
        {
            DebugLog($"Wait for condition has timeout");
            await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_timeoutOut)));
        }
        else
        {
            DebugLog($"Condition passed. Continuing flow");
            await base.ContinueFlow(handler, inPort);
        }
    }

    private void OnConditionUpdated()
    {
        m_isConditionPassed = m_condition.Test();
    }

    public override void Dispose()
    {
        base.Dispose();
        m_condition.Dispose();
    }
}