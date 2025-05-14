using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(350)]
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
        m_condition.Initialize();
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort port)
    {
        DebugLog($"Waiting for condition");
        
        OnConditionUpdated();
        
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
        
        UniTask task = UniTask.WaitUntil(TestInterruption, PlayerLoopTiming.Update, cancellationToken);
        
        if (await task.SuppressCancellationThrow())
        {
            DebugLog($"Wait is interrupted");

            if (!m_timeoutSource.IsCancellationRequested)
            {
                DebugLog($"Pause/stop requested");
                // The graph is being paused => We have to wait its reactivation
                await Execute(handler);
            }
        }

        if (m_timeoutSource is { IsCancellationRequested: true })
        {
            DebugLog($"Wait is timeout");
            await ContinueFlow(handler, GetOutputPort(nameof(m_timeoutOut)));
        }
        else
        {
            DebugLog($"Continuing flow");
            await ContinueFlow(handler);        
        }
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