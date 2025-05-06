using System;
using System.Collections;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;
using XNode.Odin;

public abstract class BaseNode : SerializableNode
{
    public virtual void Initialize() { }

    // public abstract UniTask Execute(CancellationToken stopToken, Func<CancellationToken> pauseToken, Func<CancellationToken> resumeToken);
    public virtual async UniTask Execute(GraphRunnerHandler handler)
    {
        if (handler.PauseToken.IsCancellationRequested)
        {
            Debug.Log($"[{name}] pause requested");
            await UniTask.WaitUntilCanceled(handler.ResumeToken);
            Debug.Log($"[{name}] resumed");
        }

        if (handler.StopToken.IsCancellationRequested)
        {
            Debug.Log($"[{name}] stop requested");
            throw new OperationCanceledException(handler.StopToken);
        }
    }

    // protected async UniTask ContinueFlow(CancellationToken stopToken, Func<CancellationToken> pauseToken, Func<CancellationToken> resumeToken)
    protected async UniTask ContinueFlow(GraphRunnerHandler handler)
    {
        NodePort outputPort = GetOutputPort("m_out");
        UniTask[] tasks = new UniTask[outputPort.ConnectionCount];
        int index = 0;
        foreach (NodePort otherPort in outputPort.GetConnections())
        {
            BaseNode nextNode = otherPort.node as BaseNode;
            if (nextNode != null)
            {
                // tasks[index] = nextNode.Execute(stopToken, pauseToken, resumeToken);
                tasks[index] = nextNode.Execute(handler);
                index++;
            }
        }

        await UniTask.WhenAll(tasks);
    }
}
