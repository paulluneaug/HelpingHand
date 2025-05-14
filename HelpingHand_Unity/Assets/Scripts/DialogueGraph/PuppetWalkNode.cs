using System.Diagnostics;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

using Debug = UnityEngine.Debug;

public class PuppetWalkNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;
    
    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private float m_duration;

    [SerializeField] [LabelWidth(125)]
    private bool m_waitForCompletion = false;

    private PuppetWalk m_puppet;

    public override void Initialize()
    {
        m_puppet = PuppetWalk.Instance;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort port)
    {
        if (m_waitForCompletion)
        {
            await WalkAsync(handler);
        }
        else
        {
            WalkAsync(handler).Forget();
        }
        await ContinueFlow(handler);
    }

    private async UniTask WalkAsync(GraphRunnerHandler handler)
    {
        await WalkAsyncFor(handler, m_duration);
    }

    private async UniTask WalkAsyncFor(GraphRunnerHandler handler, float duration)
    {
        m_puppet.StartWalk();
        Stopwatch timer = new ();
        timer.Start();
        if (await UniTask.WaitForSeconds(duration, false, PlayerLoopTiming.TimeUpdate, handler.StopToken).SuppressCancellationThrow())
        {
            timer.Stop();
            DebugLog($"Interrupted");
            // Test if paused
            if (handler.PauseToken.IsCancellationRequested)
            {
                Debug.Log($"{Debug_GetLogHeader()} Pause requested. Walked for {timer.ElapsedMilliseconds / 1000f} seconds");
                m_puppet.StopWalk();
                await UniTask.WaitUntilCanceled(handler.ResumeToken);
                // Time left to walk?
                float timeLeft = duration - (timer.ElapsedMilliseconds / 1000f);
                Debug.Log($"{Debug_GetLogHeader()} Resumed. Left to walk {timeLeft} seconds");
                await WalkAsyncFor(handler, timeLeft);
                return;
            }
            else // Cancelled => need to move back?
            {
                
            }
        }
        m_puppet.StopWalk();
    }
}