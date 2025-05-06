using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[NodeWidth(300)]
public class ConditionNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private PreconditionBase m_condition;

    public override void Initialize()
    {
        m_condition.Initialize();
    }

    public override async UniTask Execute(GraphRunnerHandler handler)
    {
        await base.Execute(handler);
        
        if (m_condition.Test())
        {
            await ContinueFlow(handler);
        }
    }

    public override object GetValue(NodePort port)
    {
        return m_out;
    }
}