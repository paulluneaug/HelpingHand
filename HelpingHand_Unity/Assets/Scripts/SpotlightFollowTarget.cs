using UnityEngine;

public class SpotlightFollowTarget : MonoBehaviour
{
    [SerializeField]
    private Transform m_target;

    private Transform m_transform;

    private void Awake()
    {
        m_transform = transform;
    }

    private void Update()
    {
        m_transform.LookAt(m_target);
    }
}
