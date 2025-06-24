using System;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[CreateNodeMenu("Graph/Controls")]
[NodeTint(0.6078432f, 0.2627451f, 0.6235294f)]
[NodeWidth(200)]
public class GraphControlNode : BaseNode
{
    [Input]
    [SerializeField]
    private DialogueFlow m_in;

    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [Input(ShowBackingValue.Never)]
    [ShowInInspector]
    private GraphRunnerHandler m_handlerIn;

    [Output]
    [ShowInInspector]
    private GraphRunnerHandler m_handlerOut;

    [SerializeField]
    [HideLabel]
    [HideIf("@GetInputPort(\"m_handlerIn\").ConnectionCount > 0")]
    private SimpleGraph m_graph;

    [SerializeField]
    [Space]
    [HideLabel]
    [EnumToggleButtons]
    private GraphControlsEnum m_control;

    [SerializeField]
    [LabelWidth(125)]
    [ShowIf("@m_control == GraphControlsEnum.Start")]
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
        if (m_handlerOut != null)
        {
            DebugLog($"[{GetInstanceID()}] Found handler: {m_handlerOut.GraphRunner.Graph.name}");
        }
        else
        {
            Debug.Assert(m_graph != null);
            DebugLog($"[{GetInstanceID()}] Using graph: {m_graph.name}");
        }

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
        DebugLog($"[{GetInstanceID()}] PlayGraph");
        if (m_waitForCompletion)
        {
            if (m_handlerOut == null)
            {
                GraphRunner runner = await GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.CreateGraphRunner(m_graph);
                m_handlerOut = runner.Handler;
            }
            DebugLog($"[{GetInstanceID()}] Running {m_handlerOut.GraphRunner.Graph.name}");
            await m_handlerOut.GraphRunner.RunGraphAsync().AttachExternalCancellation(handler.StopToken);
        }
        else
        {
            if (m_handlerOut == null)
            {
                GraphRunner runner = await GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.CreateGraphRunner(m_graph);
                m_handlerOut = runner.Handler;
            }
            DebugLog($"[{GetInstanceID()}] Running {m_handlerOut.GraphRunner.Graph.name}");
            m_handlerOut.GraphRunner.RunGraphAsync().Forget();
        }
    }

    private void StopGraph()
    {
        if (m_handlerOut == null)
        {
            if (GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.TryGetGraphRunner(m_graph, out GraphRunner runner))
            {
                m_handlerOut = runner.Handler;
            }
        }

        if (m_handlerOut != null)
        {
            DebugLog($"[{GetInstanceID()}] Stopping {m_handlerOut.GraphRunner.Graph.name}");
            m_handlerOut.GraphRunner.StopGraph();
        }
        else
        {
            DebugLog($"StopGraph: no graph provided", LogType.Error);
        }
    }

    private void PauseGraph()
    {
        if (m_handlerOut == null)
        {
            if (GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.TryGetGraphRunner(m_graph, out GraphRunner runner))
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
            if (GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.TryGetGraphRunner(m_graph, out GraphRunner runner))
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
            DebugLog($"{Debug_GetLogHeader()} ResumeGraph: no graph provided", LogType.Error);
        }
    }

    private async UniTaskVoid CreateGraphRunnerAndForget()
    {
        DebugLog($"CreateGraphRunnerAndForget");
        GraphRunner runner = await GameManager.Instance.ActSequenceManager.CurrentAct.GraphController.CreateGraphRunner(m_graph);
        m_handlerOut = runner.Handler;
        runner.RunGraphAsync().Forget();
    }

    public void OnApplicationQuit()
    {
        m_handlerOut = null;
        m_handlerIn = null;
    }
}