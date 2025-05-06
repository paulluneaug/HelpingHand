using System;
using System.Collections;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

[CreateAssetMenu]
public class SimpleGraph : NodeGraph
{
    private StartNode m_startNode;

    public void Initialize()
    {
        foreach (Node node in nodes)
        {
            if (node is BaseNode nodeBase)
            {
                nodeBase.Initialize();
            }

            if (node is StartNode startNode)
            {
                m_startNode = startNode;
            }
        }
    }

    // public async UniTask Run(CancellationToken stopToken, Func<CancellationToken> pauseToken, Func<CancellationToken> resumeToken)
    public async UniTask Run(GraphRunnerHandler handler)
    {
        // await m_startNode.Execute(stopToken, pauseToken, resumeToken);
        await m_startNode.Execute(handler);
    }
}