using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

public class DialogueSequenceNode : BaseNode
{
    [Input] [SerializeField]
    private DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple)]
    public List<DialogueFlow> m_sequence = new();

    [Output][ShowIf("@m_loop == false")]
    public DialogueFlow m_else;

    [SerializeField]
    private bool m_loop;

    private NodePort[] m_orderedNodePorts;
    private int m_sequenceIndex;

    protected override void Init()
    {
        base.Init();
        m_description = "Continue le flow vers le premier dialogue de la séquence qui n'a pas été lu";
    }

    public override void Initialize()
    {
        m_sequenceIndex = -1;
        m_orderedNodePorts = DynamicOutputs.OrderBy(p => p.fieldName).ToArray();
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        List<NodePort> continuePorts = new();

        m_sequenceIndex++;
        DebugLog($"sequenceIndex={m_sequenceIndex} | sequenceCount={m_orderedNodePorts.Length}");
        
        if (m_loop)
        {
            m_sequenceIndex %= m_orderedNodePorts.Length;
            DebugLog($"is lopping | sequenceIndex={m_sequenceIndex}");
        }
        
        if (m_sequenceIndex < m_orderedNodePorts.Length)
        {
            NodePort outputPort = m_orderedNodePorts[m_sequenceIndex];

            foreach (BaseNode node in GetConnectedNodesToPort(outputPort))
            {
                if (node is DialogueNode dialogueNode)
                {
                    if (dialogueNode.ReadCount == 0 || dialogueNode.MultipleReads)
                    {
                        continuePorts.Add(outputPort);
                        break;
                    }
                }
            }
        }
        else
        {
            continuePorts.Add(GetOutputPort(nameof(m_else)));
        }

        DebugLog($"Reading Dialogues: [{continuePorts.Select(port => GetConnectedNodesToPort(port).Select(node => (node as DialogueNode).name).Aggregate((dialogue, aggregate) => $"{aggregate}, {dialogue}")).Aggregate((dialogues, aggregate) => $"{aggregate}, {dialogues}")}]");
        await UniTask.WhenAll(continuePorts.Select(port => ContinueFlow(handler, inPort, port)));
    }
}