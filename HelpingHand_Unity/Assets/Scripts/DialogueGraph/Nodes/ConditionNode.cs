using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[NodeWidth(350)]
public class ConditionNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private ConditionBase m_condition;

    public override void Initialize()
    {
        m_condition.Initialize();
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        if (m_condition.Test())
        {
            await base.ContinueFlow(handler, inPort);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        m_condition.Dispose();
    }
}