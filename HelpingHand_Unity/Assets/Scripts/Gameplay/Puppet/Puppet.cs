using UnityEngine;

using UnityUtility.Extensions;
using UnityUtility.MathU;

public class Puppet : MonoBehaviour
{
    public PuppetSettings Settings => m_puppetSettings;

    [SerializeField] private PuppetSettings m_puppetSettings;
    [SerializeField] private PuppetBehaviour m_puppetBehaviour;

    [SerializeField] private Transform m_defaultParent;
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private CapsuleCollider m_collider;

    private void Start()
    {
        m_puppetBehaviour.StartBehaviour(this);
    }

    private void Update()
    {
        m_puppetBehaviour.UpdateBehaviour(Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");

        if (collision.transform.TryGetComponent(out ParentPuppetTrigger parentTrigger))
        {
            transform.parent = parentTrigger.Parent;
            m_rigidbody.linearVelocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.TryGetComponent(out ParentPuppetTrigger parentTrigger))
        {
            transform.parent = m_defaultParent;
            m_rigidbody.linearVelocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
        }
    }

    #region Puppet Movement
    public void MoveForward(float distance)
    {

        Vector3 point0 = m_collider.center - Vector3.up * (m_collider.height / 2 - m_collider.radius);
        Vector3 point1 = m_collider.center + Vector3.up * (m_collider.height / 2 - m_collider.radius);

        if (Physics.CapsuleCast(point0, point1, m_collider.radius, transform.forward, distance))
        {
            return;
        }

        m_rigidbody.MovePosition(m_rigidbody.position + transform.forward * distance);
    }

    public void SetPosition(Vector3 position, bool teleport = false)
    {
        if (teleport)
        {
            m_rigidbody.MovePosition(position);
            return;
        }

        Vector3 offset = position - m_rigidbody.position;

        Vector3 point0 = m_rigidbody.position + m_collider.center - Vector3.up * (m_collider.height / 2 - m_collider.radius);
        Vector3 point1 = m_rigidbody.position + m_collider.center + Vector3.up * (m_collider.height / 2 - m_collider.radius);

        if (Physics.CapsuleCast(point0, point1, m_collider.radius, offset, offset.magnitude))
        {
            return;
        }


        m_rigidbody.MovePosition(position);
    }

    /// <param name="angle">In radians</param>
    public void Rotate(float angle)
    {
        transform.Rotate(Vector3.up, angle * MathUf.RAD_2_DEG);
    }

    /// <param name="angle">In radians</param>
    public void SetRotationY(float angle)
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.WhereY(angle * MathUf.RAD_2_DEG));
    }
    #endregion
}
