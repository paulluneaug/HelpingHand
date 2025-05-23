using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[CreateNodeMenu("Flow/Simple/Condition")] [NodeWidth(350)] [NodeTint(0.4f, 0.2f, 0f)]
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