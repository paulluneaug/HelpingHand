using Sirenix.OdinInspector;

using UnityEngine;

public class PuppetWalk : MonoBehaviour
{
    private static readonly int s_isWalking = Animator.StringToHash("IsWalking");

    [SerializeField] private float m_walkSpeed;
    [SerializeField] private bool m_doMove = true;
    [SerializeField] private Vector3 m_walkDirection = new(1, 0, 0);
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidbody;

    private bool IsWalking => m_animator.GetBool(s_isWalking);

    private void Awake()
    {
        StopWalk();
    }

    private void Update()
    {
        if (!m_doMove)
        {
            m_rigidbody.linearVelocity = Physics.gravity;
            return;
        }
        if (IsWalking)
        {
            m_rigidbody.linearVelocity = m_walkDirection * m_walkSpeed;
        }
        else
        {
            m_rigidbody.linearVelocity = Physics.gravity;
        }
    }

    [DisableIf("IsWalking")]
    [HorizontalGroup("Split", 0.5f)]
    [Button("Start walk", ButtonSizes.Small)]
    private void StartWalk()
    {
        m_animator.SetBool(s_isWalking, true);
    }

    [EnableIf("IsWalking")]
    [HorizontalGroup("Split", 0.5f)]
    [Button("Stop walk", ButtonSizes.Small)]
    private void StopWalk()
    {
        m_animator.SetBool(s_isWalking, false);
    }
}
