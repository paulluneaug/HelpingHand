using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using XNode;

[NodeWidth(350)]
public class SwitchSequenceNode : BaseNode
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
        m_description = "Continue le flow vers le premier noeud dont la condition est vraie";
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

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort port)
    {
        await ContinueFlow(handler);
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler)
    {
        foreach (NodePort outputPort in DynamicOutputs.OrderBy(p => p.fieldName))
        {
            if (GetValue(outputPort) is ConditionBase condition)
            {
                if (condition.Test())
                {
                    await ContinueFlow(handler, outputPort);
                    return;
                }
            }
            else
            {
                throw new InvalidCastException($"{Debug_GetLogHeader()}");
            }
        }

        NodePort elsePort = GetOutputPort("m_else");
        await ContinueFlow(handler, elsePort);
    }
}