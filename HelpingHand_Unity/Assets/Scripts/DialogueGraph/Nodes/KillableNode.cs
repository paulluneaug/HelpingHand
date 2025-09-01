using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public abstract class KillableNode : BaseNode
{
    [Space]
    [SerializeField]
    [Input]
    private DialogueFlow m_kill;

    [Input]
    public DialogueFlow m_resetKill;

    protected bool m_hasBeenKilled;

    protected CancellationTokenSource m_killCTS;
    protected CancellationTokenSource m_killStopCTS;

    public override void Initialize()
    {
        m_hasBeenKilled = false;
        m_killCTS = new();
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        if (inPort.fieldName.Equals(nameof(m_kill)))
        {
            DebugLog($"Killing node");
            m_hasBeenKilled = true;
            m_killCTS.Cancel();
        }
        else if (inPort.fieldName.Equals(nameof(m_resetKill)))
        {
            DebugLog($"Reset kill status");
            m_hasBeenKilled = false;
            RenewCTS(handler.StopToken);
        }
        else
        {
            RenewCTS(handler.StopToken);
        }

        await base.ExecuteNode(handler, inPort);
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        if (m_hasBeenKilled)
        {
            DebugLog($"Killed! Stopping here");
            return;
        }

        await base.ContinueFlow(handler, inPort);
    }

    private void RenewCTS(CancellationToken stopToken)
    {
        DebugLog($"Kill CTS renewed");
        m_killCTS?.Dispose();
        m_killCTS = new CancellationTokenSource();
        m_killStopCTS?.Dispose();
        m_killStopCTS = CancellationTokenSource.CreateLinkedTokenSource(m_killCTS.Token, stopToken);
    }

    public override void Dispose()
    {
        base.Dispose();
        m_killCTS?.Dispose();
        m_killStopCTS?.Dispose();
    }
}