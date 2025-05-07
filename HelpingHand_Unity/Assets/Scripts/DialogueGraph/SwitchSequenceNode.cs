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
    public List<PreconditionBase> m_conditions = new();

    [Output]
    public DialogueFlow m_else;

    protected override void Init()
    {
        base.Init();
        m_description = "Continue le flow vers le premier noeud dont la condition est vraie";
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

    public override void Initialize()
    {
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        await ContinueFlow(handler);
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler)
    {
        foreach (NodePort outputPort in DynamicOutputs.OrderBy(p => p.fieldName))
        {
            PreconditionBase condition = GetValue(outputPort) as PreconditionBase;
            if (condition.Test())
            {
                await ContinueFlow(handler, outputPort);
                return;
            }
        }

        NodePort elsePort = GetOutputPort("m_else");
        await ContinueFlow(handler, elsePort);
    }
}