using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;
using XNode.Odin;

public abstract class BaseNode : SerializableNode
{
    public virtual void Initialize() { }

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

    protected virtual async UniTask ContinueFlow(GraphRunnerHandler handler)
    {
        NodePort outputPort = GetOutputPort("m_out");
        await ContinueFlow(handler, outputPort);
    }

    protected virtual async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort outputPort)
    {
        List<UniTask> tasks = new ();
        foreach (NodePort otherPort in outputPort.GetConnections())
        {
            BaseNode nextNode = otherPort.node as BaseNode;
            if (nextNode != null)
            {
                tasks.Add(nextNode.Execute(handler));
            }
        }

        await UniTask.WhenAll(tasks);
    }
}
