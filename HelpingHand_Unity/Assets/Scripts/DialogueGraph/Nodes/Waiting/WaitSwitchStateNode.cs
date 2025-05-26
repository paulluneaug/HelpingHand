using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(350)]
[CreateNodeMenu("Waiting/Wait Any State")]
[NodeTint(0.2f, 0.1f, .3f)]
public class WaitSwitchStateNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    public List<EntityState> m_states = new();

    [Space]
    [SerializeField]
    private bool m_doesTimeout;

    [Output]
    [ShowIf(nameof(m_doesTimeout))]
    public DialogueFlow m_timeoutOut;

    [SerializeField]
    [ShowIfGroup(nameof(m_doesTimeout))]
    private float m_timeout;

    private List<NodePort> m_continuePortList;
    private CancellationTokenSource m_timeoutSource;
    private readonly bool m_isTimeout;

    private EntityState GetState(NodePort port)
    {
        if (int.TryParse(port.fieldName[9..], out int index))
        {
            return m_states[index];
        }

        throw new ArgumentOutOfRangeException($"{Debug_GetLogHeader()} wrong fieldname ({port.fieldName})");
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        while (true)
        {
            DebugLog($"Waiting for states");

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

            m_continuePortList = new();
            UniTask task = UniTask.WaitUntil(() =>
            {
                m_continuePortList.Clear();
                foreach (NodePort outputPort in DynamicOutputs)
                {
                    EntityState state = GetState(outputPort);
                    if (state.IsSet)
                    {
                        m_continuePortList.Add(outputPort);
                    }
                }

                return m_continuePortList.Count > 0;
            }, PlayerLoopTiming.Update, cancellationToken);

            if (await task.SuppressCancellationThrow())
            {
                DebugLog($"Wait is interrupted");

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
        if (m_timeoutSource is { IsCancellationRequested: true })
        {
            DebugLog($"Wait is timeout");
            await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_timeoutOut)));
        }
        else
        {
            DebugLog($"Set states has been found: [{m_continuePortList.Select(port => (GetState(port)).name).Aggregate((state, aggregate) => $"{aggregate}, {state}")}]");
            await UniTask.WhenAll(m_continuePortList.Select(port => ContinueFlow(handler, inPort, port)));
        }
    }
}