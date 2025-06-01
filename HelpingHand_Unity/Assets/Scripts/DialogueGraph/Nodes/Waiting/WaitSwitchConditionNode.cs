using System;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Utils;

using XNode;

[NodeWidth(350)]
[CreateNodeMenu("Waiting/Wait Any Condition")]
[NodeTint(0.2f, 0.1f, .3f)]
public class WaitSwitchConditionNode : WaitNodeBase
{
    [Space]
    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    [SerializeField]
    private List<ConditionBase> m_conditions = new();

    private readonly Dictionary<ConditionBase, bool> m_conditionTestsDictionary = new();
    private readonly Dictionary<ConditionBase, NodePort> m_conditionPortsDictionary = new();
    private readonly Dictionary<ConditionBase, Action> m_conditionActionsDictionary = new();
    private readonly PriorityQueue<NodePort, int> m_continuePortQueue = new(Comparer<int>.Create((i1, i2) => -i1.CompareTo(i2)));
    private CancellationTokenSource m_timeoutSource;
    private readonly bool m_isTimeout;

    protected override string Infos => "Attend et continue le flow vers le premier port dont la condition est vraie";

    public override void Initialize()
    {
        base.Initialize();
        foreach (ConditionBase condition in m_conditions)
        {
            condition.Initialize();
            m_conditionTestsDictionary[condition] = condition.Test();
            ConditionBase c = condition;
            m_conditionActionsDictionary[c] = () => OnConditionUpdated(c);
        }

        foreach (NodePort outputPort in DynamicOutputs)
        {
            ConditionBase condition = GetCondition(outputPort);
            m_conditionPortsDictionary[condition] = outputPort;
        }
    }
    
    private void OnConditionUpdated(ConditionBase condition)
    {
        DebugLog($"OnConditionUpdated");
        m_conditionTestsDictionary[condition] = condition.Test();
    }
    
    protected override void InitializeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        foreach (ConditionBase condition in m_conditions)
        {
            condition.OnPreconditionUpdated += m_conditionActionsDictionary[condition];
        }
    }

    protected override void DisposeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        foreach (ConditionBase condition in m_conditions)
        {
            condition.OnPreconditionUpdated -= m_conditionActionsDictionary[condition];
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

    protected override void UpdateWaitUntilTest()
    {
        m_continuePortQueue.Clear();
        bool found = false;
        foreach (ConditionBase condition in m_conditions)
        {
            if (m_conditionTestsDictionary[condition])
            {
                m_continuePortQueue.Enqueue(m_conditionPortsDictionary[condition], condition.Score());
                found = true;
            }
        }

        m_stopWait = found;
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        if (IsTimeout)
        {
            DebugLog($"Has timeout");
            await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_timeoutOut)));
        }
        else
        {
            await ContinueFlow(handler, inPort, m_continuePortQueue.Peek());
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
}