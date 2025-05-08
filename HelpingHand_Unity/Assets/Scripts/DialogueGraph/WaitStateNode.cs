using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public class WaitStateNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private EntityState m_state;

    public override object GetValue(NodePort port)
    {
        return m_out;
    }

    public override void Initialize()
    {
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        if (await UniTask.WaitUntil(() => m_state.IsSet, PlayerLoopTiming.Update, handler.StopToken).SuppressCancellationThrow())
        {
            Debug.Log($"{Debug_GetLogHeader()} Wait interrupted");
            // The graph is being paused => We have to wait its reactivation
            await Execute(handler);
        }

        await ContinueFlow(handler);
    }
}