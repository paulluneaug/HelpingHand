using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

public abstract class WaitNodeBase : InterruptableNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;
    
    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [Space]
    [LabelText("Timeout?")]
    [SerializeField]
    private bool m_doesTimeout;

    [Output]
    [ShowIf(nameof(m_doesTimeout))]
    public DialogueFlow m_timeoutOut;

    [SerializeField]
    [ShowIf(nameof(m_doesTimeout))]
    private float m_timeout;

    [SerializeField] 
    [ShowIf(nameof(m_doesTimeout))]
    [LabelText("Loop after timeout?")]
    private bool m_loopAfterTimeout;
    
    private CancellationTokenSource m_timeoutSource;
    
    protected bool m_stopWait;
    protected bool IsTimeout => m_timeoutSource is { IsCancellationRequested: true };

    public override void Initialize()
    {
        base.Initialize();
        m_stopWait = false;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        await base.ExecuteNode(handler, inPort);
        if (m_hasBeenKilled)
        {
            return;
        }
        
        if (m_loopAfterTimeout)
        {
            while (!m_hasBeenKilled)
            {
                await ExecuteNodeInternal(handler, inPort);
                if (IsTimeout)
                {
                    DebugLog($"Has timeout");
                    await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_timeoutOut)));
                    handler.CurrentNodes.Add(this);
                    
                    DebugLog($"Looping...");
                }
                else
                {
                    break; 
                }
            }
        }
        else
        {
            await ExecuteNodeInternal(handler, inPort);
        }
    }

    protected virtual void InitializeExecute(GraphRunnerHandler handler, NodePort inPort) { }
    
    protected virtual void DisposeExecute(GraphRunnerHandler handler, NodePort inPort) { }

    protected virtual bool WaitUntilTest()
    {
        return m_stopWait;
    }

    protected virtual void UpdateWaitUntilTest() { }

    private async UniTask ExecuteNodeInternal(GraphRunnerHandler handler, NodePort inPort)
    {
        InitializeExecute(handler, inPort);

        while (!m_hasBeenKilled)
        {
            await base.ExecuteNode(handler, inPort);
            if (m_hasBeenKilled)
            {
                break;
            }
            
            UpdateWaitUntilTest();

            CancellationToken cancellationToken = m_killStopCTS.Token;

            if (m_doesTimeout)
            {
                DebugLog($"With timeout ({m_timeout} seconds)");

                m_timeoutSource?.Dispose();
                m_timeoutSource = new();
                m_timeoutSource.CancelAfterSlim(TimeSpan.FromSeconds(m_timeout));
                CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(m_killStopCTS.Token, m_timeoutSource.Token);
                cancellationToken = linkedTokenSource.Token;
            }

            UniTask task = UniTask.WaitUntil(WaitUntilTest, PlayerLoopTiming.Update, cancellationToken);

            if (await task.SuppressCancellationThrow())
            {
                DebugLog($"Wait interrupted");

                if (!m_doesTimeout || !m_timeoutSource.IsCancellationRequested)
                {
                    DebugLog($"Killed/Paused/Stopped");
                    // The graph is being paused => We have to wait its reactivation
                    await HandlePauseStop(handler);
                    continue;
                }
            }

            break;
        }
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        DisposeExecute(handler, inPort);

        if (m_hasBeenKilled)
        {
            return;
        }
        
        if (IsTimeout)
        {
            DebugLog($"Has timeout");
            await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_timeoutOut)));
        }
        else
        {
            DebugLog($"Continue");
            await base.ContinueFlow(handler, inPort);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        m_timeoutSource?.Dispose();
    }
}