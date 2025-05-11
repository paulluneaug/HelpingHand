using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(350)]
public class WaitAnyStateNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    public List<EntityState> m_states = new();

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

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == nameof(m_timeoutOut))
        {
            return new DialogueFlow { active = m_doesTimeout && m_timeoutSource.IsCancellationRequested };
        }
        else
        {
            return base.GetValue(port);
        }
    }

    private EntityState GetState(NodePort port)
    {
        if (int.TryParse(port.fieldName[9..], out int index))
        {
            return m_states[index];
        }

        throw new ArgumentOutOfRangeException(port.fieldName);
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        DebugLog($"Waiting for states");

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

        List<NodePort> continuePorts = new();
        UniTask task = UniTask.WaitUntil(() =>
        {
            continuePorts.Clear();
            foreach (NodePort outputPort in DynamicOutputs)
            {
                EntityState state = GetState(outputPort);
                if (state.IsSet)
                {
                    continuePorts.Add(outputPort);
                }
            }
            return continuePorts.Count > 0;
        }, PlayerLoopTiming.Update, cancellationToken);
        
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
            DebugLog($"Set states has been found: [{continuePorts.Select(port => (GetState(port)).name).Aggregate((state, aggregate) => $"{aggregate}, {state}")}]");
            await UniTask.WhenAll(continuePorts.Select(port => ContinueFlow(handler, port)));
        }
    }
}