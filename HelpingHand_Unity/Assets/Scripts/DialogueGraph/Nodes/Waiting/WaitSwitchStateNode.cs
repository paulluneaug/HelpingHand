using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[NodeWidth(350)]
[CreateNodeMenu("Waiting/Wait Any State")]
[NodeTint(0.2f, 0.1f, .3f)]
public class WaitSwitchStateNode : WaitNodeBase
{
    [Space]
    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    [SerializeField]
    private List<EntityState> m_states = new();

    private List<NodePort> m_continuePortList;
    private readonly bool m_isTimeout;

    public override void Initialize()
    {
        base.Initialize();
        m_continuePortList = new List<NodePort>();
    }

    protected override void InitializeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        foreach (EntityState state in m_states)
        {
            state.RemoveListener(UpdateWaitUntilTest);
            state.AddListener(UpdateWaitUntilTest);
        }
    }

    protected override void DisposeExecute(GraphRunnerHandler handler, NodePort inPort)
    {
        foreach (EntityState state in m_states)
        {
            state.RemoveListener(UpdateWaitUntilTest);
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        foreach (EntityState state in m_states)
        {
            state.RemoveListener(UpdateWaitUntilTest);
        }
    }

    protected override void UpdateWaitUntilTest()
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

        m_stopWait = m_continuePortList.Count > 0;
    }

    private EntityState GetState(NodePort port)
    {
        if (int.TryParse(port.fieldName[9..], out int index))
        {
            return m_states[index];
        }

        throw new ArgumentOutOfRangeException($"{Debug_GetLogHeader()} wrong fieldname ({port.fieldName})");
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
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
            DebugLog($"Set states has been found: [{m_continuePortList.Select(port => (GetState(port)).name).Aggregate((state, aggregate) => $"{aggregate}, {state}")}]");
            await UniTask.WhenAll(m_continuePortList.Select(port => ContinueFlow(handler, inPort, port)));
        }
    }
}