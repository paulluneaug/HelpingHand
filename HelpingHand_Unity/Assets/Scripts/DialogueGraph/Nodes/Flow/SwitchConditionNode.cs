using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Flow/Multiple/Switch Condition")]
[NodeWidth(350)]
[NodeTint(0.4f, 0.2f, 0f)]
public class SwitchConditionNode : BaseNode
{
    private enum SwitchType
    {
        [LabelText("All matching")] All,
        [LabelText("First one")] First
    }

    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    [SerializeField]
    [PropertySpace(8, 8)]
    private List<ConditionBase> m_conditions = new();

    [Output]
    [SerializeField]
    private DialogueFlow m_else;

    [SerializeField]
    [EnumToggleButtons]
    private SwitchType m_type;

    private IEnumerable<NodePort> m_outputPorts;

    public override void Initialize()
    {
        foreach (ConditionBase condition in m_conditions)
        {
            condition.Initialize();
        }

        m_outputPorts = DynamicOutputs.OrderBy(p => p.fieldName);
    }

    private ConditionBase GetCondition(NodePort port)
    {
        if (int.TryParse(port.fieldName[13..], out int index))
        {
            return m_conditions[index];
        }
        throw new ArgumentOutOfRangeException($"{Debug_GetLogHeader()} wrong fieldname ({port.fieldName})");
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        List<NodePort> continuePorts = new();
        foreach (NodePort outputPort in m_outputPorts)
        {
            ConditionBase condition = GetCondition(outputPort);
            if (condition.Test())
            {
                if (outputPort.GetConnections().Count > 0)
                {
                    continuePorts.Add(outputPort);
                }
            }
        }

        if (continuePorts.Count > 0)
        {
            switch (m_type)
            {
                case SwitchType.All:
                    await UniTask.WhenAll(continuePorts.Select(port => ContinueFlow(handler, inPort, port)));
                    break;
                case SwitchType.First:
                    await ContinueFlow(handler, inPort, continuePorts[0]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        else
        {
            NodePort outputPort = GetOutputPort(nameof(m_else));
            await ContinueFlow(handler, inPort, outputPort);
        }
    }
}