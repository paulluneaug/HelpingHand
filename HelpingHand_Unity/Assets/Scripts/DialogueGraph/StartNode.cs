using Cysharp.Threading.Tasks;

using XNode;

public class StartNode : BaseNode
{
    [Output]
    public DialogueFlow m_out;

    protected override void Init()
    {
        base.Init();
        m_description = "Start the graph here";
    }

    public override void Initialize()
    {
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        await ContinueFlow(handler);
    }
}