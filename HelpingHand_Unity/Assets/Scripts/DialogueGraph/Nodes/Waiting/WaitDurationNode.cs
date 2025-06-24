using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;
[CreateNodeMenu("Waiting/Wait Duration")]
[NodeTint(0.2f, 0.1f, .3f)]
public class WaitDurationNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    [LabelWidth(100)]
    private float m_waitTime;

    [SerializeField]
    [LabelWidth(100)]
    private bool m_unscaled = false;

    protected override string Infos => "Wait for n seconds before resuming the flow. Loops back if interrupted";

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        await base.ExecuteNode(handler, inPort);
        if (m_hasBeenKilled)
        {
            DebugLog($"Killed! skipping");
            return;
        }

        DebugLog($"Waiting for {m_waitTime} seconds");
        if (await UniTask.WaitForSeconds(m_waitTime, m_unscaled, PlayerLoopTiming.Update, m_killStopCTS.Token).SuppressCancellationThrow())
        {
            DebugLog($"Wait interrupted");
            if (m_hasBeenKilled)
            {
                DebugLog($"Killed! skipping");
                return;
            }

            DebugLog($"Paused / Stopped");
            // The graph is being paused => We have to wait its reactivation
            await HandlePauseStop(handler);
            await ExecuteNode(handler, inPort);
        }

        DebugLog($"Wait done");
    }
}