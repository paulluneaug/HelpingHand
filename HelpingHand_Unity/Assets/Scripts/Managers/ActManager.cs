using System;
using System.Diagnostics;

using UnityEngine;

public class ActManager : MonoBehaviour
{
    public bool IsFinished => m_isFinished;
    public GraphController GraphController => m_graphController;

    [SerializeField] private GraphController m_graphController;
    [SerializeField] private bool m_autoStart = false;

    // Cache
    [NonSerialized] private bool m_isFinished;

    private void Start()
    {
        if (m_autoStart)
        {
            GameManager.Instance.ActSequenceManager.RegisterAct(this);
        }
    }

    // For debug
    [Conditional("UNITY_EDITOR")]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.ActSequenceManager.RegisterAct(this);
        }
    }

    public void StartAct(Puppet puppet)
    {
        m_isFinished = false;
        puppet.gameObject.SetActive(true);

        m_graphController.OnGraphSequenceFinished += OnGraphSequenceFinished;
        m_graphController.StartSequence();
    }

    private void OnGraphSequenceFinished()
    {
        m_graphController.OnGraphSequenceFinished -= OnGraphSequenceFinished;
        m_isFinished = true;
        // Play credits
        GameManager.Instance.ReturnToMainMenu();
        GameManager.Instance.CanvasManager.OpenCredits();
    }

    public void Dispose()
    {
        m_graphController.StopAllGraphs();
    }

    public void PauseAct()
    {
        m_graphController.PauseGraphs();
    }

    public void ResumeAct()
    {
        m_graphController.ResumeGraphs();
    }
}
