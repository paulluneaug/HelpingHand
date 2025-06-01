using System;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;
[CreateNodeMenu("Waiting/Wait Frame")] [NodeTint(0.2f, 0.1f, .3f)]
public class WaitFrameNode : InterruptableNode
{
    private enum WaitType
    {
        [LabelText("Pre LateUpdate")] PreLateUpdate,
        [LabelText("Post LateUpdate")] PostLateUpdate,
        [LabelText("End of frame")] EndOfFrame,
        [LabelText("Next frame")] NextFrame,
        [LabelText("Frame count")] FrameCount,
    }
    
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField] [LabelWidth(50)]
    private WaitType m_type;

    [SerializeField] [LabelWidth(100)] [ShowIf("@m_type == WaitType.FrameCount")]
    private int m_frameCount;

    protected override string Infos => "Wait for n seconds before resuming the flow. Loops back if interrupted";
    
    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        while (true)
        {
            UniTask task;
            switch (m_type)
            {
                case WaitType.PreLateUpdate:
                    task = UniTask.Yield(PlayerLoopTiming.PreLateUpdate, handler.StopToken);
                    break;
                case WaitType.PostLateUpdate:
                    task = UniTask.Yield(PlayerLoopTiming.PostLateUpdate, handler.StopToken);
                    break;
                case WaitType.EndOfFrame:
                    DebugLog($"Waiting for end of frame");
                    task = UniTask.WaitForEndOfFrame(handler.StopToken); 
                    break;
                case WaitType.NextFrame:
                    DebugLog($"Waiting for next frame");
                    task = UniTask.NextFrame(handler.StopToken);
                    break;
                case WaitType.FrameCount:
                    DebugLog($"Waiting for {m_frameCount} frames");
                    task = UniTask.DelayFrame(m_frameCount, PlayerLoopTiming.Update, handler.StopToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (await task.SuppressCancellationThrow())
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