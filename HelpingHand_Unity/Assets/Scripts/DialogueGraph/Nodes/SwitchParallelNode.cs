using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using XNode;

[NodeWidth(350)]
public class SwitchParallelNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    public List<ConditionBase> m_conditions = new();

    [Output]
    public DialogueFlow m_else;

    protected override void Init()
    {
        base.Init();
        m_description = "Continue le flow vers tous les noeuds dont la condition est vraie";
    }

    public override void Initialize()
    {
        foreach (ConditionBase condition in m_conditions)
        {
            condition.Initialize();
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

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        List<NodePort> continuePorts = new();
        foreach (NodePort outputPort in DynamicOutputs)
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
            await UniTask.WhenAll(continuePorts.Select(port => ContinueFlow(handler, inPort, port)));
        }
        else
        {
            NodePort outputPort = GetOutputPort(nameof(m_else));
            await ContinueFlow(handler, inPort, outputPort);
        }
    }
}