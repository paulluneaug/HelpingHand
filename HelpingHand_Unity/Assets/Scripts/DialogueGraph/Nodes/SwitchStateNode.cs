using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using XNode;

[NodeWidth(350)]
public class SwitchStateNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    public List<EntityState> m_states = new();

    [Output]
    public DialogueFlow m_else;

    private int m_caseCount = 0;

    protected override void Init()
    {
        base.Init();
        m_description = "Continue le flow vers tous les noeuds dont l'état est set";
    }

    public override void Initialize()
    {
        m_caseCount = 0;
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
        foreach (NodePort outputPort in DynamicOutputs)
        {
            EntityState state = GetState(outputPort);
            if (state.IsSet)
            {
                continuePorts.Add(outputPort);
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