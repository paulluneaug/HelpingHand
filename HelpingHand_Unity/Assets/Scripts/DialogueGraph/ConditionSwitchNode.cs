using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(300)]
public class ConditionSwitchNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    public List<PreconditionBase> m_conditions = new();

    [Output]
    public DialogueFlow m_else;

    private int m_caseCount = 0;

    public override void Initialize()
    {
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler)
    {
        List<UniTask> tasks = new();
        bool found = false;
        foreach (NodePort outputPort in DynamicOutputs)
        {
            PreconditionBase condition = GetValue(outputPort) as PreconditionBase;
            if (condition.Test())
            {
                found = true;
                tasks.Add(ContinueFlow(handler, outputPort));
            }
        }

        if (found)
        {
            await UniTask.WhenAll(tasks);
        }
        else
        {
            NodePort outputPort = GetOutputPort("m_else");
            await ContinueFlow(handler, outputPort);
        }
    }

    public override async UniTask Execute(GraphRunnerHandler handler)
    {
        await base.Execute(handler);

        await ContinueFlow(handler);
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "m_else")
        {
            return m_else;
        }
        else
        {
            int index = int.Parse(port.fieldName[13..]);
            return m_conditions[index];
        }
    }
}