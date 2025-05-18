using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(350)]
public class InterruptWithConditionNode : InterruptableNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField] [HideLabel]
    private ConditionBase m_condition = new ConditionAnd();
    
    private bool m_isConditionPassed;
    private bool m_canInterrupt;
    
    public override void Initialize()
    {
        m_isConditionPassed = false;
        m_condition.Initialize();
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        while (true)
        {
            DebugLog($"Waiting for condition");

            OnConditionUpdated();

            UniTask task = UniTask.WaitUntil(TestInterruption, PlayerLoopTiming.Update, handler.StopToken);

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
            if (await GraphManager.Instance.Interrupt(handler))
            {
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
        return m_isConditionPassed && GraphManager.Instance.CurrentNodeCanBeInterrupted;
    }

    private void OnConditionUpdated()
    {
        m_isConditionPassed = m_condition.Test();
    }

    private void OnDestroy()
    {
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
    }
}