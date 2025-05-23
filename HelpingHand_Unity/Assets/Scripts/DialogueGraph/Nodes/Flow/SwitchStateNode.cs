using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Flow/Multiple/Switch State")] [NodeWidth(350)] [NodeTint(0.4f, 0.2f, 0f)]
public class SwitchStateNode : BaseNode
{
    private enum SwitchType
    {
        [LabelText("All matching")] All,
        [LabelText("First one")] First
    }
    
    [Input] [SerializeField]
    private DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)] [SerializeField] [PropertySpace(8,8)]
    private List<EntityState> m_states = new();

    [Output] [SerializeField]
    private DialogueFlow m_else;

    [SerializeField] [EnumToggleButtons]
    private SwitchType m_type;

    private int m_caseCount = 0;
    private IEnumerable<NodePort> m_outputPorts;

    public override void Initialize()
    {
        m_caseCount = 0;
        m_outputPorts = DynamicOutputs.OrderBy(p => p.fieldName);
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
        List<NodePort> continuePorts = new();
        foreach (NodePort outputPort in m_outputPorts)
        {
            EntityState state = GetState(outputPort);
            if (state.IsSet)
            {
                continuePorts.Add(outputPort);
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