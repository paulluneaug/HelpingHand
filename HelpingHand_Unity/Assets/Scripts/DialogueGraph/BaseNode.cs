using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;
using XNode.Odin;

public abstract class BaseNode : SerializableNode
{
    [SerializeField]
    [HideLabel][FoldoutGroup("Description")][TextArea(1, 2)]
    protected string m_description;
    
    public virtual void Initialize() { }

    public async UniTask Execute(GraphRunnerHandler handler)
    {
        await HandlePauseStop(handler);
        await ExecuteNode(handler);
    }

    protected abstract UniTask ExecuteNode(GraphRunnerHandler handler);

    private async UniTask HandlePauseStop(GraphRunnerHandler handler)
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
        foreach (BaseNode nextNode in GetConnectedNodesToPort(outputPort))
        {
            if (nextNode is DialogueNode dialogueNode)
            {
                if (dialogueNode.CanBeReadMultipleTimes || !dialogueNode.HasBeenRead)
                {
                    tasks.Add(nextNode.Execute(handler));
                }
            }
            else
            {
                tasks.Add(nextNode.Execute(handler));
            }
        }

        await UniTask.WhenAll(tasks);
    }

    protected BaseNode[] GetConnectedNodesToPort(NodePort port)
    {
        return port.GetConnections().Select(p => p.node as BaseNode).Where(n => n != null).ToArray();
    }
}
