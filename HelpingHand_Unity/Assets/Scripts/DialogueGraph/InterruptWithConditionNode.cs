using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[NodeWidth(350)]
public class InterruptWithConditionNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private PreconditionBase m_condition;

    private bool m_isConditionPassed;

    protected override void Init()
    {
        base.Init();
        m_description = "Wait for the condition to be true & the current dialogue to be interruptable";
    }

    public override void Initialize()
    {
        m_isConditionPassed = false;
        m_condition.Initialize();
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
    }

    public override object GetValue(NodePort port)
    {
        return port.fieldName switch
        {
            nameof(m_out) => m_out,
            nameof(m_in) => m_in,
            _ => null
        };
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        Debug.Log($"{Debug_GetLogHeader()} Waiting for condition");
        OnConditionUpdated();
        if (await UniTask.WaitUntil(TestInterruption, PlayerLoopTiming.Update, handler.StopToken).SuppressCancellationThrow())
        {
            Debug.Log($"{Debug_GetLogHeader()} Wait interrupted");
            return;
        }

        Debug.Log($"{Debug_GetLogHeader()} Interrupting");

        GraphManager.Instance.Interrupt(handler);

        await UniTask.NextFrame();
        await ContinueFlow(handler);
    }

    private bool TestInterruption()
    {
        return GraphManager.Instance.CurrentNodeCanBeInterrupted && m_isConditionPassed;
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