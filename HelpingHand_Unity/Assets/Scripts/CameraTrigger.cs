using Unity.Cinemachine;

using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera m_camera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_camera.Priority = 1;
            m_camera.Prioritize();
        }
    }
}
