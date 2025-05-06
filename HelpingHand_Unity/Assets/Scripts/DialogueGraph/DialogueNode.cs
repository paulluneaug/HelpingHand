using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[NodeWidth(200)]
public class DialogueNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;
    
    [Output]
    public DialogueFlow m_out;

    [TextArea(3, 3)]
    public string m_content;

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
        
        Debug.Log(m_content);
        // await ContinueFlow(stopToken, pauseToken, resumeToken);
        await ContinueFlow(handler);
    }
}