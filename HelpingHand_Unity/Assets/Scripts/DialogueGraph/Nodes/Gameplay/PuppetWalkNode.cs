using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

public class PuppetWalkNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    public DialogueFlow m_out;

    [SerializeField]
    [LabelWidth(125)]
    private bool m_waitForEndOfSpline = false;

    private Puppet m_puppet;

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        m_puppet = GameManager.Instance.GetPuppet();
        if (m_waitForEndOfSpline)
        {
            await WalkAsync(handler);
        }
        else
        {
            WalkAsync(handler).Forget();
        }
    }

    private async UniTask WalkAsync(GraphRunnerHandler handler)
    {
        DebugLog($"BeginWalk");
        m_puppet.BeginWalk();
        await HandleCancellation(handler);
        DebugLog($"StopWalk");
        m_puppet.StopWalk();
    }

    private async UniTask HandleCancellation(GraphRunnerHandler handler)
    {
        if (await UniTask.WaitUntil(() => m_puppet.HasReachedEndOfSpline, PlayerLoopTiming.TimeUpdate, handler.StopToken).SuppressCancellationThrow())
        {
            DebugLog($"Interrupted");
            // Test if paused
            if (handler.PauseToken.IsCancellationRequested)
            {
                m_puppet.PauseWalk();
                await UniTask.WaitUntilCanceled(handler.ResumeToken);

                m_puppet.ResumeWalk();
                await HandleCancellation(handler);
                return;
            }
            else // Cancelled => need to move back?
            {

            }
        }
        DebugLog($"End of spline reached");
    }
}