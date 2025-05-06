using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public class WaitNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;
    
    [Output]
    public DialogueFlow m_out;

    public float m_waitTime;
    public bool m_unscaled = false;

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
        
        bool isCanceled =  await UniTask.WaitForSeconds(m_waitTime, m_unscaled, PlayerLoopTiming.Update, handler.StopToken).SuppressCancellationThrow();
        if (isCanceled)
        {
            Debug.Log($"WaitNode: wait cancelled");
        }
        
        // await ContinueFlow(stopToken, pauseToken, resumeToken);
        await ContinueFlow(handler);
    }
}