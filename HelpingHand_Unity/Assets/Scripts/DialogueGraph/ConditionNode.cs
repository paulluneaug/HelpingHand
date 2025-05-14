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

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort port)
    {
        if (m_condition.Test())
        {
            await ContinueFlow(handler);
        }
    }
}