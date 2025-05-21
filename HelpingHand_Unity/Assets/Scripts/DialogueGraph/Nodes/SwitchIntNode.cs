using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(350)]
public class SwitchIntNode : BaseNode
{
    [Input] [SerializeField]
    private DialogueFlow m_in;

    [Input] [SerializeField] [PropertySpace(8, 8)]
    private int m_value;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)] [SerializeField]
    private List<int> m_values = new();

    [SerializeField] [PropertySpace(8)] 
    private bool m_exactValues;
    
    [Output] [PropertySpace(8)] [ShowIf("@m_exactValues == true")]
    public DialogueFlow m_else;

    [ShowInInspector] [ReadOnly] 
    private int Value => m_value;

    private SortedSet<int> m_sortedValues = new();
    private Dictionary<int, NodePort> m_portsDictionary = new();
    
    public override void Initialize()
    {
        base.Initialize();
        foreach (NodePort outputPort in DynamicOutputs)
        {
            int value = GetValue(outputPort);
            m_portsDictionary[value] = outputPort;
            m_sortedValues.Add(value);
        }
    }

    private new int GetValue(NodePort port)
    {
        if (int.TryParse(port.fieldName[9..], out int index))
        {
            return m_values[index];
        }
        throw new ArgumentOutOfRangeException($"{Debug_GetLogHeader()} wrong fieldname ({port.fieldName})");
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        if (this.TryGetValueFromInputPort(nameof(m_value), out int outValue))
        {
            m_value = outValue;
        }

        int portNumber = m_value;
        bool found = false;

        // Look for the closest lower or equal number in the keys
        if (!m_exactValues)
        {
            foreach (int v in m_sortedValues)
            {
                if (v <= m_value)
                {
                    portNumber = v;
                    found = true;
                }
                else
                {
                    break;
                }
            }
        }

        if ((m_exactValues || found) && m_portsDictionary.TryGetValue(portNumber, out NodePort outputPort))
        {
            await ContinueFlow(handler, inPort, outputPort);
        }
        else
        {
            NodePort elsePort = GetOutputPort(nameof(m_else));
            await ContinueFlow(handler, inPort, elsePort);
        }
    }
}