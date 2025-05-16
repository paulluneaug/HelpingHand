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
    /// Initialize the node when the graph starts. Use this to reset the node state (non serialized variables)
    /// </summary>
    public abstract void Initialize();
    
    /// <summary>
    /// Returns the value coming from the port specified. Override this if the node has value port. It is not
    /// necessary for flow nodes.
    /// </summary>
    public override object GetValue(NodePort port)
    {
        return new DialogueFlow();
    }

    /// <summary>
    /// Execute the node with a flow coming from the default input port: m_in
    /// </summary>
    public async UniTask Execute(GraphRunnerHandler handler)
    {
        // By default the input port is m_in
        NodePort inputPort = GetInputPort("m_in");
        await Execute(handler, inputPort);
    }

    /// <summary>
    /// Execute the node with a flow coming from the input port specified
    /// </summary>
    public async UniTask Execute(GraphRunnerHandler handler, NodePort port)
    {
        handler.CurrentNode = this;
        await HandlePauseStop(handler);
        await ExecuteNode(handler, port);
        // await ContinueFlow(handler); // we want this
    }
    
    /// <summary>
    /// Execute the node with a flow coming from the input port specified. 
    /// </summary>
    protected abstract UniTask ExecuteNode(GraphRunnerHandler handler,  NodePort port);

    /// <summary>
    /// Handles the pausing, resuming and stopping of the graph.
    /// </summary>
    private async UniTask HandlePauseStop(GraphRunnerHandler handler)
    {
        // If the graph is paused, either pause token & stop token are triggered
        if (handler.PauseToken.IsCancellationRequested)
        {
            DebugLog($"Pause requested");
            await UniTask.WaitUntilCanceled(handler.ResumeToken);
            DebugLog($"Resumed");
        }

        // If it's only stopped, only the stop token is triggered
        if (handler.StopToken.IsCancellationRequested)
        {
            DebugLog($"Stop requested");
            throw new OperationCanceledException(handler.StopToken);
        }
    }

    /// <summary>
    /// Continue the flow by the default out port m_out.
    /// It has to be called at the end of the execution.
    /// It has to be overriden if the flow can divert to multiple out ports.
    /// </summary>
    protected virtual async UniTask ContinueFlow(GraphRunnerHandler handler)
    {
        // By default the out port is m_out
        NodePort outputPort = GetOutputPort("m_out");
        await ContinueFlow(handler, outputPort);
    }

    /// <summary>
    /// Continue the flow by the port specified.
    /// It has to be called at the end of the execution.
    /// It has to be overriden if the flow can divert to multiple out ports.
    /// </summary>
    protected virtual async UniTask ContinueFlow(GraphRunnerHandler handler, NodePort outputPort)
    {
        // All the resulting flows
        List<UniTask> tasks = new ();
        
        // Get all connections of the output port. For each one, execute the connected node.
        // Wait for all the executions to be finished before returning
        foreach (NodePort nextPort in outputPort.GetConnections())
        {
            if (nextPort.node is BaseNode nextNode)
            {
                // A dialogue node is a special case.
                // We don't want to consume the flow if the dialogue cannot be read
                if (nextNode is DialogueNode dialogueNode)
                {
                    if (dialogueNode.MultipleReads || !dialogueNode.HasBeenRead.Value)
                    {
                        tasks.Add(nextNode.Execute(handler, nextPort));
                    }
                }
                else
                {
                    tasks.Add(nextNode.Execute(handler, nextPort));
                }
            }
            else
            {
                DebugLog($"Next node is not a {nameof(BaseNode)}: {nextPort.node.name}", LogType.Error);
                throw new InvalidCastException($"Next node is not a {nameof(BaseNode)}: {nextPort.node.name}");
            }
        }

        await UniTask.WhenAll(tasks);
    }

    /// <summary>
    /// Returns all the nodes connected to the specified port.
    /// </summary>
    protected BaseNode[] GetConnectedNodesToPort(NodePort port)
    {
        return port.GetConnections().Select(p => p.node as BaseNode).Where(n => n != null).ToArray();
    }

    /// <summary>
    /// Debug log header
    /// </summary>
    protected string Debug_GetLogHeader()
    {
        return $"[{Time.frameCount}] <color=cyan>[{graph.name}]</color> <color=yellow>[{GetType().Name}]</color> ({name})";
    }

    /// <summary>
    /// Debug log with header
    /// TODO: move it project-wise 
    /// </summary>
    protected void DebugLog(string log, LogType logType = LogType.Log, GameObject source = null)
    {
        switch (logType)
        {
            case LogType.Error:
                Debug.LogError($"{Debug_GetLogHeader()} {log}", source);
                break;
            case LogType.Warning:
                Debug.LogWarning($"{Debug_GetLogHeader()} {log}", source);
                break;
            case LogType.Log:
                Debug.Log($"{Debug_GetLogHeader()} {log}", source);
                break;
        }
    }
}
