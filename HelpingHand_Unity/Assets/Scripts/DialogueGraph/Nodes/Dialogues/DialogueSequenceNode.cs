using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Dialogues/Dialogue Sequence")]
[NodeTint(0f, 0.2f, 0f)]
[NodeWidth(250)]
public class DialogueSequenceNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple)]
    public List<DialogueFlow> m_sequence = new();

    [Output]
    [ShowIf("@m_loop == false")]
    public DialogueFlow m_else;

    [SerializeField]
    private bool m_loop;

    [SerializeField]
    private bool m_random;

    private NodePort[] m_orderedNodePorts;
    private int m_sequenceIndex;
    private Queue<int> m_randomQueue;

    protected override string Infos => "Continue le flow vers le premier dialogue de la séquence qui n'a pas été lu.\n" +
                                       "Peut boucler après avoir lu tous les dialogues (ou sort dans \"else\").\n" +
                                       "Peut choisir les ports séquentiellement ou au hasard.";

    public override void Initialize()
    {
        m_sequenceIndex = -1;
        m_orderedNodePorts = DynamicOutputs.OrderBy(p => p.fieldName).ToArray();
        InitializeRandomQueue();
    }

    private void InitializeRandomQueue()
    {
        System.Random rnd = new(Guid.NewGuid().GetHashCode());
        m_randomQueue = new Queue<int>(Enumerable.Range(0, m_orderedNodePorts.Length).OrderBy(r => rnd.Next()));
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        List<NodePort> continuePorts = new();

        if (TryGetNextSequenceIndex(ref m_sequenceIndex))
        {
            NodePort outputPort = m_orderedNodePorts[m_sequenceIndex];

            foreach (BaseNode node in GetConnectedNodesToPort(outputPort))
            {
                if (node is DialogueNode dialogueNode)
                {
                    if (dialogueNode.ReadCount == 0 || dialogueNode.CanRepeat)
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

    private bool TryGetNextSequenceIndex(ref int sequenceIndex)
    {
        if (m_random)
        {
            if (m_randomQueue.TryDequeue(out sequenceIndex))
            {
                return true;
            }
            else
            {
                if (m_loop)
                {
                    InitializeRandomQueue();
                    sequenceIndex = m_randomQueue.Dequeue();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            sequenceIndex++;
            if (m_loop)
            {
                sequenceIndex %= m_orderedNodePorts.Length;
            }

            return sequenceIndex < m_orderedNodePorts.Length;
        }
    }
}