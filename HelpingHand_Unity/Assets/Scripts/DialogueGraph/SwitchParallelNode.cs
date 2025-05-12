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

    private int m_caseCount = 0;

    protected override void Init()
    {
        base.Init();
        m_description = "Continue le flow vers tous les noeuds dont la condition est vraie";
    }

    public override void Initialize()
    {
    }
    
    public override object GetValue(NodePort port)
    {
        if (int.TryParse(port.fieldName[13..], out int index))
        {
            return m_conditions[index];
        }
        else
        {
            throw new ArgumentOutOfRangeException($"{Debug_GetLogHeader()} wrong fieldname ({port.fieldName})");
        }
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler)
    {
        List<NodePort> continuePorts = new();
        foreach (NodePort outputPort in DynamicOutputs)
        {
            ConditionBase condition = GetValue(outputPort) as ConditionBase;
            if (condition.Test())
            {
                continuePorts.Add(outputPort);
            }
        }

        if (continuePorts.Count > 0)
        {
            await UniTask.WhenAll(continuePorts.Select(port => ContinueFlow(handler, port)));
        }
        else
        {
            NodePort outputPort = GetOutputPort("m_else");
            await ContinueFlow(handler, outputPort);
        }
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        await ContinueFlow(handler);
    }
}