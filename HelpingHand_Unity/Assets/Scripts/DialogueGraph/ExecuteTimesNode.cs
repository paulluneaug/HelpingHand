using System;

using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public class ExecuteTimesNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private int m_times = 1;

    private int m_executionCount;

    public override object GetValue(NodePort port)
    {
        return port.fieldName switch
        {
            "m_in" => m_in,
            "m_out" => m_out,
            "m_count" => m_times,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    protected override void Init()
    {
        base.Init();
        m_description = "Continue the flow for the maximum number of times";
    }

    public override void Initialize()
    {
        m_executionCount = 0;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        if (m_executionCount < m_times)
        {
            m_executionCount++;
            await ContinueFlow(handler);
        }
    }
}