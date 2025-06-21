using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

public class StartSimonNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [Input, SerializeField]
    private SimonSequenceVariable m_sequence;

    [Output(ShowBackingValue.Never)]
    [SerializeField]
    private bool m_sucess;

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        m_sucess = false;
        m_sucess = await HandleCancellation(handler, GameManager.Instance.SimonManager.StartSequence(m_sequence.Value));
    }

    private async UniTask<bool> HandleCancellation(GraphRunnerHandler handler, UniTask<bool> task)
    {
        (bool cancelled, bool result) = await task.SuppressCancellationThrow();
        if (cancelled)
        {
            DebugLog($"Interrupted");
            // Test if paused
            if (handler.PauseToken.IsCancellationRequested)
            {
                await UniTask.WaitUntilCanceled(handler.ResumeToken);

                return await HandleCancellation(handler, GameManager.Instance.SimonManager.ResumeSequence());
            }
            else // Cancelled => need to move back?
            {
                return false;
            }
        }
        return result;
    }
}