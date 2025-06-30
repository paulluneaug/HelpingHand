using System;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

public class PuppetWalkNode : BaseNode
{
    public enum Action
    {
        StartWalk,
        StopWalk,
        PauseWalk,
        ResumeWalk,
    }
    
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [SerializeField]
    private Action m_action;

    [SerializeField]
    [ShowIf("@m_action == PuppetWalkNode.Action.StartWalk")]
    [LabelWidth(125)]
    private bool m_waitForEndOfSpline = false;
    
    private Puppet m_puppet;

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        m_puppet = GameManager.Instance.GetPuppet();
        switch (m_action)
        {
            case Action.StartWalk:
                await StartWalk(handler);
                break;
            case Action.StopWalk:
                StopWalk(handler).Forget();
                break;
            case Action.PauseWalk:
                PauseWalk(handler).Forget();
                break;
            case Action.ResumeWalk:
                ResumeWalk(handler).Forget();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async UniTask StartWalk(GraphRunnerHandler handler)
    {
        if (m_waitForEndOfSpline)
        {
            await WalkAsync(handler);
        }
        else
        {
            WalkAsync(handler).Forget();
        }
    }

    private async UniTaskVoid StopWalk(GraphRunnerHandler handler)
    {
        m_puppet.StopWalk();
    }

    private async UniTaskVoid PauseWalk(GraphRunnerHandler handler)
    {
        m_puppet.PauseWalk();
    }

    private async UniTaskVoid ResumeWalk(GraphRunnerHandler handler)
    {
        m_puppet.ResumeWalk();
    }

    private async UniTask WalkAsync(GraphRunnerHandler handler)
    {
        DebugLog($"Starting puppet walk");
        m_puppet.BeginWalk();
        await HandleCancellation(handler);
        DebugLog($"Puppet finished walking");
        m_puppet.StopWalk();
    }

    private async UniTask HandleCancellation(GraphRunnerHandler handler)
    {
        if (await UniTask.WaitUntil(() => m_puppet.HasReachedEndOfSpline, PlayerLoopTiming.Update, handler.StopToken).SuppressCancellationThrow())
        {
            DebugLog($"Interrupted");
            
            // Node has been disposed
            if (m_isDisposed)
            {
                DebugLog($"Node disposed, going to the end of the spline");
                m_puppet.GoToEndOfSpline();
                m_puppet.StopWalk();
                throw new OperationCanceledException();
            }
            
            // Test if paused
            if (handler.PauseToken.IsCancellationRequested)
            {
                DebugLog($"Pausing puppet walk");
                m_puppet.PauseWalk();
                await UniTask.WaitUntilCanceled(handler.ResumeToken);
                DebugLog($"Resuming puppet walk");
                m_puppet.ResumeWalk();
                await HandleCancellation(handler);
                return;
            }
            
            if (handler.StopToken.IsCancellationRequested) // Stopped => need to move back?
            {
                DebugLog($"Stopping puppet walk");
                m_puppet.StopWalk();
                throw new OperationCanceledException(handler.StopToken);
            }
        }
        DebugLog($"End of spline reached");
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        await UniTask.NextFrame(handler.StopToken);
        await base.ContinueFlow(handler, inPort);
    }

    private bool m_isDisposed;

    public override void Dispose()
    {
        base.Dispose();
        m_isDisposed = true;
    }
}