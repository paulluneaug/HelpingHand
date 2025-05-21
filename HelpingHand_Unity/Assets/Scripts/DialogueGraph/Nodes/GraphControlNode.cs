using System;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Graph/Controls")] [NodeTint(0.6078432f, 0.2627451f, 0.6235294f)] [NodeWidth(200)]
public class GraphControlNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;
    
    [Output]
    public DialogueFlow m_out;

    [Input(ShowBackingValue.Never)] [ShowInInspector]
    private GraphRunnerHandler m_handlerIn;

    [Output] [ShowInInspector]
    private GraphRunnerHandler m_handlerOut;

    [SerializeField] [HideLabel] [HideIf("@GetInputPort(\"m_handlerIn\").GetInputValue<GraphRunnerHandler>() != null")]
    private SimpleGraph m_graph;

    [SerializeField] [Space] [HideLabel] [EnumToggleButtons]
    private GraphControlsEnum m_control;

    [SerializeField] [LabelWidth(125)] [ShowIf("@m_control == GraphControlsEnum.Start")]
    private bool m_waitForCompletion;

    private enum GraphControlsEnum
    {
        [LabelText("", SdfIconType.Play)] Start,
        [LabelText("", SdfIconType.ArrowRepeat)] Resume,
        [LabelText("", SdfIconType.Pause)] Pause,
        [LabelText("", SdfIconType.Stop)] Stop,
    }

    public override object GetValue(NodePort port)
    {
        return m_handlerOut;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        m_handlerOut = GetInputPort(nameof(m_handlerIn)).GetInputValue<GraphRunnerHandler>();
        
        switch (m_control)
        {
            case GraphControlsEnum.Start:
                await PlayGraph(handler);
                break;
            case GraphControlsEnum.Stop:
                StopGraph();
                break;
            case GraphControlsEnum.Pause:
                PauseGraph();
                break;
            case GraphControlsEnum.Resume:
                ResumeGraph();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async UniTask PlayGraph(GraphRunnerHandler handler)
    {
        if (m_waitForCompletion)
        {
            if (m_handlerOut == null)
            {
                GraphRunner runner = await GraphManager.Instance.CreateGraphRunner(m_graph);
                m_handlerOut = runner.Handler;
            }
            await m_handlerOut.GraphRunner.RunGraphAsync().AttachExternalCancellation(handler.StopToken);
        }
        else
        {
            if (m_handlerOut == null)
            {
                CreateGraphRunnerAndForget().Forget();
            }
            else
            {
                m_handlerOut.GraphRunner.RunGraphAsync().Forget();
            }
        }
    }

    private void StopGraph()
    {
        if (m_handlerOut == null)
        {
            if (GraphManager.Instance.TryGetGraphRunner(m_graph, out GraphRunner runner))
            {
                m_handlerOut = runner.Handler;
            }
        }

        if (m_handlerOut != null)
        {
            m_handlerOut.GraphRunner.StopGraph();
        }
        else
        {
            Debug.LogError($"{Debug_GetLogHeader()} StopGraph: no graph provided");
        }
    }

    private void PauseGraph()
    {
        if (m_handlerOut == null)
        {
            if (GraphManager.Instance.TryGetGraphRunner(m_graph, out GraphRunner runner))
            {
                m_handlerOut = runner.Handler;
            }
        }

        if (m_handlerOut != null)
        {
            m_handlerOut.GraphRunner.PauseGraph();
        }
        else
        {
            Debug.LogError($"{Debug_GetLogHeader()} PauseGraph: no graph provided");
        }
    }

    private void ResumeGraph()
    {
        if (m_handlerOut == null)
        {
            if (GraphManager.Instance.TryGetGraphRunner(m_graph, out GraphRunner runner))
            {
                m_handlerOut = runner.Handler;
            }
        }

        if (m_handlerOut != null)
        {
            m_handlerOut.GraphRunner.ResumeGraph();
        }
        else
        {
            Debug.LogError($"{Debug_GetLogHeader()} ResumeGraph: no graph provided");
        }
    }

    private async UniTaskVoid CreateGraphRunnerAndForget()
    {
        GraphRunner runner = await GraphManager.Instance.CreateGraphRunner(m_graph);
        m_handlerOut = runner.Handler;
        runner.RunGraphAsync().Forget();
    }

    public void OnApplicationQuit()
    {
        m_handlerOut = null;
        m_handlerIn = null;
    }
}