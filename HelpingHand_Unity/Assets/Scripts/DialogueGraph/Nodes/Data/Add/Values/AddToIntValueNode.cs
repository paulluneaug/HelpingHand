using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Data/Add/Values/Int")]
[NodeTint(0f, 0.4784314f, 0.6509804f)]
[NodeWidth(250)]
public class AddToIntValueNode : BaseNode
{
    [Input(ShowBackingValue.Never)]
    [SerializeField]
    private DialogueFlow m_in;

    [Input]
    [SerializeField]
    private int m_value;

    [Input]
    [SerializeField]
    private int m_increment;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [Output]
    [SerializeField]
    private int m_valueOut;

    [ShowInInspector]
    [ReadOnly]
    private int m_currentValue;

    public override object GetValue(NodePort port)
    {
        return m_valueOut;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (this.TryGetValueFromInputPort(nameof(m_value), out int val))
        {
            m_currentValue = val;
        }
        
        if (this.TryGetValueFromInputPort(nameof(m_increment), out int increment))
        {
            m_increment = increment;
        }

        m_currentValue += m_increment;

        m_valueOut = m_currentValue;

        await UniTask.CompletedTask;
    }
}