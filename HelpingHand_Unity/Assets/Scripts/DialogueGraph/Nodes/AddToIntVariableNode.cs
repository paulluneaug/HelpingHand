using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(250)]
public class AddToIntVariableNode : BaseNode
{
    [Input(ShowBackingValue.Never)] 
    public DialogueFlow m_in;

    [Input]
    public int m_increment;

    [Output] public DialogueFlow m_out;

    [SerializeField] [InlineEditor]
    private IntVariable m_variable;

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        int inValue = m_increment;
        NodePort inValuePort = GetInputPort(nameof(m_increment));
        if (inValuePort.ConnectionCount > 0)
        {
            inValue = inValuePort.GetInputValue<int>();
            Debug.Log($"Reading value {inValue} from input port");
        }
        else
        {
            Debug.Log($"Reading value {inValue} from node");
        }

        m_variable.Value += inValue;
    }
}