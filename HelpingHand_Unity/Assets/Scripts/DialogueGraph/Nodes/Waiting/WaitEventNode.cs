using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Events;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(250)]
[CreateNodeMenu("Waiting/Wait Event")]
[NodeTint(0.2f, 0.1f, .3f)]
public class WaitEventNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    [HideLabel]
    private BaseGameEvent m_event;

    [Space]
    [SerializeField]
    private bool m_doesTimeout;

    [Output]
    [ShowIf(nameof(m_doesTimeout))]
    public DialogueFlow m_timeoutOut;

    [SerializeField]
    [ShowIfGroup(nameof(m_doesTimeout))]
    private float m_timeout;

    private CancellationTokenSource m_timeoutSource;
    private bool m_isEventRaised;

    public override void Initialize()
    {
        m_isEventRaised = false;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        m_event.RemoveListener(OnEventRaised);
        m_event.AddListener(OnEventRaised);

        while (true)
        {
            DebugLog($"Waiting for event {m_event.name}");

            CancellationToken cancellationToken = handler.StopToken;

            if (m_doesTimeout)
            {
                DebugLog($"With timeout ({m_timeout} seconds)");
                m_timeoutSource?.Dispose();
                m_timeoutSource = new();
                _ = m_timeoutSource.CancelAfterSlim(TimeSpan.FromSeconds(m_timeout));
                CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(handler.StopToken, m_timeoutSource.Token);
                cancellationToken = linkedTokenSource.Token;
            }

            UniTask task = UniTask.WaitUntil(() => m_isEventRaised, PlayerLoopTiming.Update, cancellationToken);

            if (await task.SuppressCancellationThrow())
            {
                DebugLog($"Wait for event has been interrupted");

                if (!m_timeoutSource.IsCancellationRequested)
                {
                    DebugLog($"Pause/stop requested");
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
        m_event.RemoveListener(OnEventRaised);

        if (m_timeoutSource is { IsCancellationRequested: true })
        {
            DebugLog($"Wait for event has timeout");
            await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_timeoutOut)));
        }
        else
        {
            DebugLog($"Event has been raised. Continuing flow");
            await base.ContinueFlow(handler, inPort);
        }
    }


    private void OnEventRaised()
    {
        m_isEventRaised = true;
    }

    public override void Dispose()
    {
        base.Dispose();
        m_event.RemoveListener(OnEventRaised);
    }
}