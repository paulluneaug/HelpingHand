using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;
[CreateNodeMenu("Waiting/Wait State")] [NodeTint(0.2f, 0.1f, .3f)] [NodeWidth(300)]
public class WaitStateNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    [HideLabel]
    private EntityState m_state;

    [SerializeField]
    [LabelText("Must be set?")]
    private bool m_mustBeSet = true;

    [Space]
    [LabelText("Timeout?")]
    [SerializeField]
    private bool m_doesTimeout;

    [Output]
    [ShowIf(nameof(m_doesTimeout))]
    public DialogueFlow m_timeoutOut;

    [SerializeField]
    [ShowIf(nameof(m_doesTimeout))]
    private float m_timeout;

    [SerializeField] 
    [ShowIf(nameof(m_doesTimeout))]
    [LabelText("Loop after timeout?")]
    private bool m_loopAfterTimeout;

    private CancellationTokenSource m_timeoutSource;
    private bool m_isConditionPassed;

    public override void Initialize()
    {
        m_isConditionPassed = false;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        m_state.RemoveListener(OnStateUpdated);
        m_state.AddListener(OnStateUpdated);

        while (true)
        {
            DebugLog($"Waiting for state {m_state.name}");

            OnStateUpdated();

            CancellationToken cancellationToken = handler.StopToken;

            if (m_doesTimeout)
            {
                DebugLog($"With timeout ({m_timeout} seconds)");

                m_timeoutSource?.Dispose();
                m_timeoutSource = new();
                _ = m_timeoutSource.CancelAfterSlim(TimeSpan.FromSeconds(m_timeout));
                CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(handler.StopToken, m_timeoutSource.Token);
                cancellationToken = linkedTokenSource.Token;
            }

            UniTask task = UniTask.WaitUntil(() => m_isConditionPassed, PlayerLoopTiming.Update, cancellationToken);

            if (await task.SuppressCancellationThrow())
            {
                DebugLog($"Wait interrupted");

                if (!m_timeoutSource.IsCancellationRequested)
                {
                    DebugLog($"Paused/stopped");
                    // The graph is being paused => We have to wait its reactivation
                    await HandlePauseStop(handler);
                    continue;
                }
            }

            break;
        }
    }
    
    // TODO algo pour régler la boucle qui stack overflow
    private async UniTask ExecuteNode2(GraphRunnerHandler handler, NodePort inPort)
    {
        handler.CurrentNodes.Add(this);
        await HandlePauseStop(handler);
        // Si m_loopAfterTimeout est vrai alors 
        // Tant que
        await ExecuteNode(handler, inPort);
        // est timeout
        // alors envoyer sur 
        await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_timeoutOut)));
        // puis se remettre en noeud actif
        handler.CurrentNodes.Add(this);
        // -- fin tant que
        
        // si pas timeout, envoyer sur
        await base.ContinueFlow(handler, inPort);
        
        // Sinon exécution normale
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort inPort)
    {
        m_state.RemoveListener(OnStateUpdated);

        if (m_timeoutSource is { IsCancellationRequested: true })
        {
            DebugLog($"Wait timeout");
            await ContinueFlow(handler, inPort, GetOutputPort(nameof(m_timeoutOut)));
            // If loop is active, we must loop here
            if (m_loopAfterTimeout)
            {
                await ExecuteNode(handler, inPort);
                await ContinueFlow(handler, inPort);
            }
        }
        else
        {
            DebugLog($"State {m_state.name} is set, continue");
            await base.ContinueFlow(handler, inPort);
        }
    }

    private void OnStateUpdated()
    {
        m_isConditionPassed = m_state.IsSet == m_mustBeSet;
    }

    public override void Dispose()
    {
        base.Dispose();
        m_timeoutSource?.Dispose();
    }
}