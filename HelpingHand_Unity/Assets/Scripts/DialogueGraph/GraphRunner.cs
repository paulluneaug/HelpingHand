using System;
using System.Diagnostics;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

using Debug = UnityEngine.Debug;

public class GraphRunner : MonoBehaviour
{
    public event Action OnGraphStarted;
    public event Action OnGraphEnded;
    public event Action OnGraphCancelled;
    public event Action OnGraphStopped;
    public event Action OnGraphPaused;
    public event Action OnGraphResumed;

    public GraphRunnerHandler Handler => m_graphRunnerHandler;

    public SimpleGraph Graph => m_graph;

    public bool IsInterrupted = false;

    private GraphRunnerHandler m_graphRunnerHandler;

    [SerializeField]
    private SimpleGraph m_graph;


    public void SetGraph(SimpleGraph graph)
    {
        m_graph = graph;
    }

    private void Awake()
    {
        m_graphRunnerHandler = new GraphRunnerHandler(this);
    }

    [Button("Start")]
    [ButtonGroup("Controls")]
    public void StartGraph()
    {
        if (m_graphRunnerHandler.IsRunning)
        {
            DebugLog($"[StartGraph] Graph is not running", LogType.Warning);
            return;
        }
        RunGraphAsync().Forget();
    }

    [Button("Stop")]
    [ButtonGroup("Controls")]
    public void StopGraph()
    {
        if (!m_graphRunnerHandler.IsRunning)
        {
            DebugLog($"[StopGraph] Graph is not running", LogType.Warning);
            return;
        }
        m_graphRunnerHandler.Stop();
    }

    [Button("Pause")]
    [ButtonGroup("Controls")]
    public void PauseGraph()
    {
        if (!m_graphRunnerHandler.IsRunning)
        {
            DebugLog($"[PauseGraph] Graph is not running", LogType.Warning);
            return;
        }
        if (m_graphRunnerHandler.IsPaused)
        {
            DebugLog($"[PauseGraph] Graph is already paused", LogType.Warning);
            return;
        }
        DebugLog($"Pause");
        m_graphRunnerHandler.Pause();
        OnGraphPaused?.Invoke();
    }

    [Button("Resume")]
    [ButtonGroup("Controls")]
    public void ResumeGraph()
    {
        if (!m_graphRunnerHandler.IsRunning)
        {
            DebugLog($"[PauseGraph] Graph is not running", LogType.Warning);
            return;
        }
        if (!m_graphRunnerHandler.IsPaused)
        {
            DebugLog($"[PauseGraph] Graph is not paused", LogType.Warning);
            return;
        }
        DebugLog($"Resume");
        m_graphRunnerHandler.Resume();
        OnGraphResumed?.Invoke();
    }

    public async UniTask RunGraphAsync()
    {
        if (m_graphRunnerHandler.IsRunning)
        {
            DebugLog($"[StartGraph] Graph is not running", LogType.Warning);
            return;
        }

        IsInterrupted = false;

        DebugLog($"Initialize");
        m_graph.Initialize();

        await UniTask.NextFrame();

        DebugLog($"Start");

        m_graphRunnerHandler.Start();
        OnGraphStarted?.Invoke();
        bool isCancelled = await m_graph.Run(m_graphRunnerHandler).SuppressCancellationThrow();
        if (isCancelled)
        {
            DebugLog($"Stopped prematurely");
            OnGraphCancelled?.Invoke();
        }
        else
        {
            DebugLog($"End reached");
            OnGraphEnded?.Invoke();
        }

        DebugLog($"Stopped");
        OnGraphStopped?.Invoke();

        foreach (Node node in m_graph.nodes)
        {
            if (node is IDisposable disposableNode)
            {
                disposableNode.Dispose();
            }
        }

        UniTask.Action(async () =>
        {
            await UniTask.NextFrame();
            Destroy(gameObject);
        }).Invoke();
    }

    private void OnDisable()
    {
        m_graphRunnerHandler.Dispose();
    }

#if UNITY_EDITOR
    public void OnApplicationQuit()
    {
        foreach (Node node in m_graph.nodes)
        {
            if (node is GraphControlNode controlNode)
            {
                controlNode.OnApplicationQuit();
            }
        }
    }
#endif

    /// <summary>
    /// Debug log with header
    /// TODO: move it project-wise 
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    private void DebugLog(string log, LogType logType = LogType.Log, GameObject source = null)
    {
        string GetLogHeader()
        {
            return $"[{Time.frameCount}] <color=teal>[{GetType().Name}]</color> <color=cyan>[{m_graph.name}]</color>";
        }
        switch (logType)
        {
            case LogType.Error:
                Debug.LogError($"{GetLogHeader()} {log}", source);
                break;
            case LogType.Warning:
                Debug.LogWarning($"{GetLogHeader()} {log}", source);
                break;
            case LogType.Log:
                Debug.Log($"{GetLogHeader()} {log}", source);
                break;
            case LogType.Assert:
                break;
            case LogType.Exception:
                break;
            default:
                break;
        }
    }
}