using System;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

[NodeWidth(200)]
public class GraphControlNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;
    
    [Output]
    public DialogueFlow m_out;

    [Input(ShowBackingValue.Never)] [ShowInInspector]
    private GraphRunner m_graphRunnerIn;

    [Output] [ShowInInspector]
    private GraphRunner m_graphRunnerOut;

    [SerializeField] [HideLabel]
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

    public override void Initialize()
    {
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        m_graphRunnerOut = GetInputPort(nameof(m_graphRunnerIn)).GetInputValue<GraphRunner>();
        
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
        
        await ContinueFlow(handler);
    }

    private async UniTask PlayGraph(GraphRunnerHandler handler)
    {
        if (m_waitForCompletion)
        {
            if (m_graphRunnerOut == null)
            {
                m_graphRunnerOut = await GraphManager.Instance.CreateGraphRunner(m_graph);
            }
            await m_graphRunnerOut.RunGraphAsync().AttachExternalCancellation(handler.StopToken);
        }
        else
        {
            if (m_graphRunnerOut == null)
            {
                CreateGraphRunnerAndForget().Forget();
            }
            else
            {
                m_graphRunnerOut.RunGraphAsync().Forget();
            }
        }
    }

    private void StopGraph()
    {
        if (m_graphRunnerOut == null)
        {
            GraphManager.Instance.TryGetGraphRunner(m_graph, out m_graphRunnerOut);
        }

        if (m_graphRunnerOut != null)
        {
            m_graphRunnerOut.StopGraph();
        }
        else
        {
            Debug.LogError($"{Debug_GetLogHeader()} StopGraph: no graph provided");
        }
    }

    private void PauseGraph()
    {
        if (m_graphRunnerOut == null)
        {
            GraphManager.Instance.TryGetGraphRunner(m_graph, out m_graphRunnerOut);
        }
        
        if (m_graphRunnerOut != null)
        {
            m_graphRunnerOut.PauseGraph();
        }
        else
        {
            Debug.LogError($"{Debug_GetLogHeader()} PauseGraph: no graph provided");
        }
    }

    private void ResumeGraph()
    {
        if (m_graphRunnerOut == null)
        {
            GraphManager.Instance.TryGetGraphRunner(m_graph, out m_graphRunnerOut);
        }
        
        if (m_graphRunnerOut != null)
        {
            m_graphRunnerOut.ResumeGraph();
        }
        else
        {
            Debug.LogError($"{Debug_GetLogHeader()} ResumeGraph: no graph provided");
        }
    }

    private async UniTaskVoid CreateGraphRunnerAndForget()
    {
        m_graphRunnerOut = await GraphManager.Instance.CreateGraphRunner(m_graph);
        m_graphRunnerOut.RunGraphAsync().Forget();
    }

    public void OnApplicationQuit()
    {
        m_graphRunnerIn = null;
        m_graphRunnerOut = null;
    }
}