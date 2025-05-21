using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;
[CreateNodeMenu("Waiting/Wait Duration")] [NodeTint(0.2f, 0.1f, .3f)]
public class WaitDurationNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField][LabelWidth(100)]
    private float m_waitTime;

    [SerializeField][LabelWidth(100)]
    private bool m_unscaled = false;

    protected override void Init()
    {
        base.Init();
        m_description = "Wait for n seconds before resuming the flow. Loops back if interrupted";
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        while (true)
        {
            DebugLog($"Waiting for {m_waitTime} seconds");
            if (await UniTask.WaitForSeconds(m_waitTime, m_unscaled, PlayerLoopTiming.Update, handler.StopToken).SuppressCancellationThrow())
            {
                DebugLog($"Wait interrupted");
                // The graph is being paused => We have to wait its reactivation
                await HandlePauseStop(handler);
                continue;
            }

            DebugLog($"Wait done");
            break;
        }
    }
}