using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Graph/Interrupt")]
[NodeTint(0.6078432f, 0.2627451f, 0.6235294f)]
[NodeWidth(350)]
public class InterruptWithConditionNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    [HideLabel]
    private ConditionBase m_condition = new ConditionAnd();

    private bool m_isConditionPassed;
    private readonly bool m_canInterrupt;

    public override void Initialize()
    {
        m_isConditionPassed = false;
        m_condition.Initialize();
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        while (!m_hasBeenKilled)
        {
            await base.ExecuteNode(handler, inPort);
            if (m_hasBeenKilled)
            {
                break;
            }
            
            DebugLog($"Waiting for condition");

            OnConditionUpdated();

            UniTask task = UniTask.WaitUntil(TestInterruption, PlayerLoopTiming.Update, m_killStopCTS.Token);

            if (await task.SuppressCancellationThrow())
            {
                DebugLog($"Pause/stop requested");
                // The graph is being paused => We have to wait its reactivation
                await HandlePauseStop(handler);
                continue;
            }

            DebugLog($"Trying to interrupt");

            // If the interruption didn't happen (because another graph with more priority interrupted at the same time)
            // then exit via cancelled out port
            if (await GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.Interrupt(m_condition.Score(), handler))
            {
                await UniTask.DelayFrame(1); // Needed. There is apparently a 1 frame delay when a token is cancelled, so we need to be sure to interrupt the dialogue BEFORE this continues
                DebugLog($"Is interrupting. Continuing...");
                break;
            }
            else
            {
                DebugLog($"Can't interrupt. Looping...");
                await UniTask.WaitForSeconds(.5f, true);
            }
        }
    }

    private bool TestInterruption()
    {
        return m_isConditionPassed && GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.CurrentNodeCanBeInterrupted;
    }

    private void OnConditionUpdated()
    {
        m_isConditionPassed = m_condition.Test();
    }

    public override void Dispose()
    {
        base.Dispose();
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
    }
}