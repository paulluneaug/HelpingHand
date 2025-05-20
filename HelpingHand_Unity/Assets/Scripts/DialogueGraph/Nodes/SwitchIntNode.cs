using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using XNode;

[NodeWidth(350)]
public class SwitchIntNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Input] [PropertySpace(8, 8)] [InlineEditor]
    public IntVariable m_value;

    [Output(dynamicPortList = true, backingValue = ShowBackingValue.Always, connectionType = ConnectionType.Multiple)]
    public List<int> m_values = new();

    [Output] [PropertySpace(8)]
    public DialogueFlow m_else;
    
    private Dictionary<int, NodePort> m_portsDictionary = new();
    
    public override void Initialize()
    {
        base.Initialize();
        foreach (NodePort outputPort in DynamicOutputs)
        {
            int value = GetValue(outputPort);
            m_portsDictionary[value] = outputPort;
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
        int val = m_value.Value;
        NodePort inValuePort = GetInputPort(nameof(m_value));
        if (inValuePort.ConnectionCount > 0)
        {
            if (inValuePort.TryGetInputValue(out IntVariable variable))
            {
                val = variable.Value;
            }
            else if (inValuePort.TryGetInputValue(out int v))
            {
                val = v;
            }
        }

        if (m_portsDictionary.TryGetValue(val, out NodePort outputPort))
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