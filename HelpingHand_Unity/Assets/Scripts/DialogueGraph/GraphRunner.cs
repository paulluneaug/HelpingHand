using System;

using Sirenix.OdinInspector;

using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public class GraphRunner : MonoBehaviour
{
    public event Action OnGraphStarted;
    public event Action OnGraphEnded;
    public event Action OnGraphCancelled;
    public event Action OnGraphStopped;
    public event Action OnGraphPaused;
    public event Action OnGraphResumed;

    public GraphRunnerHandler Handler => m_graphRunnerHandler;

    private GraphRunnerHandler m_graphRunnerHandler;
    
    [SerializeField]
    private SimpleGraph m_graph;

    private void Awake()
    {
        m_graphRunnerHandler = new GraphRunnerHandler(this);
    }

    private void Start()
    {
        m_graph?.Initialize();
    }

    public void Initialize(SimpleGraph graph)
    {
        m_graph = graph;
        m_graph.Initialize();
    }

    [Button("Start")]
    [ButtonGroup("Controls")]
    public void StartGraph()
    {
        if (m_graphRunnerHandler.IsRunning)
        {
            Debug.LogWarning($"{Debug_GetLogHeader()} [StartGraph] Graph is not running");
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
            Debug.LogWarning($"{Debug_GetLogHeader()} [StopGraph] Graph is not running");
            return;
        }
        m_graphRunnerHandler.Stop();
        m_graph.Initialize();
    }

    [Button("Pause")]
    [ButtonGroup("Controls")]
    public void PauseGraph()
    {
        if (!m_graphRunnerHandler.IsRunning)
        {
            Debug.LogWarning($"{Debug_GetLogHeader()} [PauseGraph] Graph is not running");
            return;
        }
        if (m_graphRunnerHandler.IsPaused)
        {
            Debug.LogWarning($"{Debug_GetLogHeader()} [PauseGraph] Graph is already paused");
            return;
        }
        Debug.Log($"{Debug_GetLogHeader()} Pause");
        m_graphRunnerHandler.Pause();
        OnGraphPaused?.Invoke();
    }

    [Button("Resume")]
    [ButtonGroup("Controls")]
    public void ResumeGraph()
    {
        if (!m_graphRunnerHandler.IsRunning)
        {
            Debug.LogWarning($"{Debug_GetLogHeader()} [PauseGraph] Graph is not running");
            return;
        }
        if (!m_graphRunnerHandler.IsPaused)
        {
            Debug.LogWarning($"{Debug_GetLogHeader()} [PauseGraph] Graph is not paused");
            return;
        }
        Debug.Log($"{Debug_GetLogHeader()} Resume");
        m_graphRunnerHandler.Resume();
        OnGraphResumed?.Invoke();
    }

    public async UniTask RunGraphAsync()
    {
        if (m_graphRunnerHandler.IsRunning)
        {
            Debug.LogWarning($"{Debug_GetLogHeader()} [StartGraph] Graph is not running");
            return;
        }
        
        Debug.Log($"{Debug_GetLogHeader()} Start");
        m_graphRunnerHandler.Start();
        OnGraphStarted?.Invoke();
        bool isCancelled = await m_graph.Run(m_graphRunnerHandler).SuppressCancellationThrow();
        if (isCancelled)
        {
            Debug.Log($"{Debug_GetLogHeader()} Stopped prematurely");
            OnGraphCancelled?.Invoke();
        }
        else
        {
            Debug.Log($"{Debug_GetLogHeader()} End reached");
            OnGraphEnded?.Invoke();
        }

        Debug.Log($"{Debug_GetLogHeader()} Stopped");
        OnGraphStopped?.Invoke();
        
        foreach (Node node in m_graph.nodes)
        {
            if (node is IDisposable disposableNode)
            {
                disposableNode.Dispose();
            }
        }
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

    private string Debug_GetLogHeader()
    {
        return $"[{Time.frameCount}] <color=teal>[{GetType().Name}]</color> <color=cyan>[{m_graph.name}]</color>";
    }
}