using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(250)]
public class AddToIntNode : BaseNode
{
    [Input(ShowBackingValue.Never)] [SerializeField]
    private DialogueFlow m_in;

    [Input(ShowBackingValue.Always)] [SerializeField] 
    private int m_increment;

    [Output] [SerializeField]
    private DialogueFlow m_out;

    [Output] [SerializeField]
    private int m_valueOut;

    [SerializeField]
    private int m_startValue;

    [ShowInInspector] [ReadOnly]
    private int m_value;

    public override void Initialize()
    {
        base.Initialize();
        m_value = m_startValue;
    }

    public override object GetValue(NodePort port)
    {
        return m_valueOut;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (this.TryGetIntFromInputPort(nameof(m_increment), out int outValue))
        {
            m_increment = outValue;
        }
        
        m_value += m_increment;

        m_valueOut = m_value;

        await UniTask.CompletedTask;
    }
}