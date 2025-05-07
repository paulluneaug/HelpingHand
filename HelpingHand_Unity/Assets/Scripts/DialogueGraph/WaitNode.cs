using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public class WaitNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private float m_waitTime;

    [SerializeField]
    private bool m_unscaled = false;

    public override object GetValue(NodePort port)
    {
        return m_out;
    }

    public override void Initialize()
    {
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        bool isCancelled = await UniTask.WaitForSeconds(m_waitTime, m_unscaled, PlayerLoopTiming.Update, handler.StopToken).SuppressCancellationThrow();
        if (isCancelled)
        {
            Debug.Log($"WaitNode: wait cancelled");
            await Execute(handler);
        }

        await ContinueFlow(handler);
    }
}