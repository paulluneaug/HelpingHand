using System;

using UnityEngine;

//using UnityUtility.TriggerObject;

public class LevelManager : MonoBehaviour
{
    public bool IsFinished => m_isFinished;

    public Vector3 EndAnchor => m_endAnchor.position;

    //[SerializeField] private TriggerObject m_levelEndTrigger;
    [SerializeField] private Transform m_startAnchor;
    [SerializeField] private Transform m_endAnchor;

    [SerializeField] private Transform m_puppetStart;


    [NonSerialized] private bool m_isFinished;

    private void Start()
    {
        GameManager.Instance.LevelSequenceManager.RegisterLevel(this);
        // m_levelEndTrigger.OnEnter += OnEndTriggerEnter;
    }

    private void OnDestroy()
    {
        // m_levelEndTrigger.OnEnter -= OnEndTriggerEnter;
    }

    public void MoveLevel(Vector3 previousLevelAnchor)
    {
        Vector3 offset = previousLevelAnchor - m_startAnchor.position;

        foreach (GameObject rootObject in gameObject.scene.GetRootGameObjects())
        {
            rootObject.transform.Translate(offset);
        }
    }

    public void StartLevel(Puppet puppet)
    {
        m_isFinished = false;
        puppet.transform.position = m_puppetStart.position;
        puppet.gameObject.SetActive(true);
    }

    private void OnEndTriggerEnter(Collider collider)
    {
        m_isFinished = true;
    }

    private void OnDrawGizmos()
    {

    }
}
