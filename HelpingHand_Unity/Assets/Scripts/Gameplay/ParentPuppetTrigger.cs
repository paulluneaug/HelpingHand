using UnityEngine;

public class ParentPuppetTrigger : MonoBehaviour
{
    [SerializeField] private Transform m_parent;

    public Transform Parent => m_parent;
}
