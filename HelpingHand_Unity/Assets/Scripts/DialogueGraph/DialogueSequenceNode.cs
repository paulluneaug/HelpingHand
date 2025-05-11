using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public class DialogueSequenceNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;
    
    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    public List<DialogueFlow> m_sequence = new();

    [Output]
    public DialogueFlow m_else;

    protected override void Init()
    {
        base.Init();
        m_description = "Continue le flow vers le premier dialogue de la séquence qui n'a pas été lu";
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
        List<NodePort> continuePorts = new();
        bool found = false;
        foreach (NodePort outputPort in DynamicOutputs.OrderBy(p => p.fieldName))
        {
            foreach (BaseNode node in GetConnectedNodesToPort(outputPort))
            {
                if (node is DialogueNode dialogueNode)
                {
                    if (dialogueNode.ReadCount == 0)
                    {
                        found = true;
                        continuePorts.Add(outputPort);
                    }
                }
            }

            if (found)
            {
                break;
            }
        }

        if (continuePorts.Count > 0)
        {
            await UniTask.WhenAll(continuePorts.Select(port => ContinueFlow(handler, port)));
        }
        else
        {
            NodePort elsePort = GetOutputPort(nameof(m_else));
            await ContinueFlow(handler, elsePort);
        }
    }
}