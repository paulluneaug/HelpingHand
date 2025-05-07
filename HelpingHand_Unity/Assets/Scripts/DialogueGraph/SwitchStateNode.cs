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
    
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "m_else")
        {
            return m_else;
        }
        else
        {
            int index = int.Parse(port.fieldName[9..]);
            return m_states[index];
        }
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler)
    {
        List<NodePort> continuePorts = new();
        foreach (NodePort outputPort in DynamicOutputs)
        {
            EntityState state = GetValue(outputPort) as EntityState;
            if (state.IsSet)
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