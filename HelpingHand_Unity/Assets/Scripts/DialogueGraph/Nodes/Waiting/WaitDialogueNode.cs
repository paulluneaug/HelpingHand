using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Waiting/Wait Dialogue")]
[NodeTint(0.2f, 0.1f, .3f)]
public class WaitDialogueNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    [HideLabel]
    private DialogueNode m_dialogue;

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

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        while (true)
        {
            DebugLog($"Waiting for dialogue {m_dialogue.name}");

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

            UniTask task = UniTask.WaitUntil(() => m_dialogue.HasBeenRead.Value, PlayerLoopTiming.Update, cancellationToken);

            if (await task.SuppressCancellationThrow())
            {
                DebugLog($"Wait interrupted");

                if (!m_timeoutSource.IsCancellationRequested)
                {
                    DebugLog($"Paused/stopped");
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
        if (m_timeoutSource is { IsCancellationRequested: true })
        {
            DebugLog($"Wait timeout");
            await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_timeoutOut)));
        }
        else
        {
            DebugLog($"Dialogue {m_dialogue.name} has been read, continue");
            await base.ContinueFlow(handler, inPort);
        }
    }
}