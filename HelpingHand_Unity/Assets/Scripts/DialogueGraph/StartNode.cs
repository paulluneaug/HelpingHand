using System;
using System.Collections;
using System.Threading;

using Cysharp.Threading.Tasks;

using XNode;

public class StartNode : BaseNode
{
    [Output(ShowBackingValue.Never, ConnectionType.Override)]
    public DialogueFlow m_out;

    public override object GetValue(NodePort port)
    {
        return m_out;
    }

    public override void Initialize()
    {
    }

    // public override async UniTask Execute(CancellationToken stopToken, Func<CancellationToken> pauseToken, Func<CancellationToken> resumeToken)
    public override async UniTask Execute(GraphRunnerHandler handler)
    {
        await base.Execute(handler);
        // await ContinueFlow(stopToken, pauseToken, resumeToken);
        await ContinueFlow(handler);
    }
}