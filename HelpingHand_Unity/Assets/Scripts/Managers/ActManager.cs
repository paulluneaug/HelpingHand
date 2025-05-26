using System;

using UnityEngine;

public class ActManager : MonoBehaviour
{
    public bool IsFinished => m_isFinished;
    public GraphController GraphController => m_graphController;

    [SerializeField] private Transform m_puppetStart;
    [SerializeField] private GraphController m_graphController;
        
    // Cache
    [NonSerialized] private bool m_isFinished;

    private void Start()
    {
        GameManager.Instance.ActSequenceManager.RegisterAct(this);
    }

    public void StartAct(Puppet puppet)
    {
        m_isFinished = false;
        puppet.transform.position = m_puppetStart.position;
        puppet.gameObject.SetActive(true);

        m_graphController.OnGraphSequenceFinished += OnGraphSequenceFinished;
    }

    private void OnGraphSequenceFinished()
    {
        m_graphController.OnGraphSequenceFinished -= OnGraphSequenceFinished;
        m_isFinished = true;
    }

    public void Dispose()
    {

    }
}
