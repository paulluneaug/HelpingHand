using System;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Data/Add/Global Variables/Float")]
[NodeTint(0f, 0.4784314f, 0.6509804f)]
[NodeWidth(250)]
public class AddToGlobalFloatNode : BaseNode
{
    private enum Operation
    {
        [LabelText("+")] Add,
        [LabelText("-")] Remove,
        [LabelText("*")] Mult,
        [LabelText("/")] Div,
    }
    
    [Input(ShowBackingValue.Never)]
    [SerializeField]
    private DialogueFlow m_in;

    [Input(ShowBackingValue.Always)]
    [SerializeField]
    private float m_increment;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [Output(ShowBackingValue.Always)]
    [SerializeField]
    private FloatVariable m_variable;

    [SerializeField] 
    [EnumToggleButtons]
    private Operation m_operation;

    public override object GetValue(NodePort port)
    {
        return m_variable;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (this.TryGetValueFromInputPort(nameof(m_increment), out float outValue))
        {
            m_increment = outValue;
        }

        switch (m_operation)
        {
            case Operation.Add:
                m_variable.Value += m_increment;
                break;
            case Operation.Remove:
                m_variable.Value -= m_increment;
                break;
            case Operation.Mult:
                m_variable.Value *= m_increment;
                break;
            case Operation.Div:
                m_variable.Value /= m_increment;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await UniTask.CompletedTask;
    }
}