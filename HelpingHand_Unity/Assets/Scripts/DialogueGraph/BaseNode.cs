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
    
    /// <summary>
    /// Run when the graph starts
    /// </summary>
    public abstract void Initialize();
    
    public override object GetValue(NodePort port)
    {
        return new DialogueFlow() { active = true };
    }

    public async UniTask Execute(GraphRunnerHandler handler)
    {
        handler.CurrentNode = this;
        await HandlePauseStop(handler);
        await ExecuteNode(handler);
    }

    protected abstract UniTask ExecuteNode(GraphRunnerHandler handler);

    private async UniTask HandlePauseStop(GraphRunnerHandler handler)
    {
        if (handler.PauseToken.IsCancellationRequested)
        {
            Debug.Log($"{Debug_GetLogHeader()} Pause requested");
            await UniTask.WaitUntilCanceled(handler.ResumeToken);
            Debug.Log($"{Debug_GetLogHeader()} Resumed");
        }

        if (handler.StopToken.IsCancellationRequested)
        {
            Debug.Log($"{Debug_GetLogHeader()} Stop requested");
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
                if (dialogueNode.MultipleReads || !dialogueNode.HasBeenRead.Value)
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

    protected string Debug_GetLogHeader()
    {
        return $"[{Time.frameCount}] <color=cyan>[{graph.name}]</color> <color=yellow>[{GetType().Name}]</color> ({name})";
    }

    protected void DebugLog(string log)
    {
        Debug.Log($"{Debug_GetLogHeader()} {log}");
    }
}
