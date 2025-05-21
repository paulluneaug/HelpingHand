using System;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using Utils;

using XNode;

[NodeWidth(350)][CreateNodeMenu("Waiting/Wait Any Condition")] [NodeTint(0.2f, 0.1f, .3f)]
public class WaitSwitchConditionNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    public List<ConditionBase> m_conditions = new();

    [Space] [SerializeField]
    private bool m_doesTimeout;

    [Output] [ShowIf(nameof(m_doesTimeout))]
    public DialogueFlow m_timeoutOut;

    [SerializeField] [ShowIfGroup(nameof(m_doesTimeout))]
    private float m_timeout;

    private Dictionary<ConditionBase, bool> m_conditionTestsDictionary = new();
    private Dictionary<ConditionBase, NodePort> m_conditionPortsDictionary = new();
    private PriorityQueue<NodePort, int> m_continuePortQueue = new(Comparer<int>.Create((i1, i2) => -i1.CompareTo(i2)));
    private CancellationTokenSource m_timeoutSource;
    private bool m_isTimeout;

    protected override void Init()
    {
        base.Init();
        m_description = "Attend et continue le flow vers le premier noeud dont la condition est vraie";
    }

    public override void Initialize()
    {
        base.Initialize();
        foreach (ConditionBase condition in m_conditions)
        {
            condition.Initialize();
            m_conditionTestsDictionary[condition] = condition.Test();
            ConditionBase c = condition;
            condition.OnPreconditionUpdated += () => m_conditionTestsDictionary[c] = c.Test();
        }

        foreach (NodePort outputPort in DynamicOutputs)
        {
            ConditionBase condition = GetCondition(outputPort);
            m_conditionPortsDictionary[condition] = outputPort;
        }
    }

    private ConditionBase GetCondition(NodePort port)
    {
        if (int.TryParse(port.fieldName[13..], out int index))
        {
            return m_conditions[index];
        }
        throw new ArgumentOutOfRangeException($"{Debug_GetLogHeader()} wrong fieldname ({port.fieldName})");
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        while (true)
        {
            DebugLog($"Waiting for conditions");

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

            UniTask task = UniTask.WaitUntil(() =>
            {
                m_continuePortQueue.Clear();
                bool found = false;
                foreach (ConditionBase condition in m_conditions)
                {
                    if (m_conditionTestsDictionary[condition])
                    {
                        m_continuePortQueue.Enqueue(m_conditionPortsDictionary[condition], condition.Depth());
                        found = true;
                    }
                }
                
                return found;
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
            DebugLog($"A condition is passed, continuing");
            await ContinueFlow(handler, inPort, m_continuePortQueue.Peek());
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        foreach (ConditionBase condition in m_conditions)
        {
            condition.Dispose();
        }
    }
}