using UnityEngine;

using UnityUtility.Extensions;
using UnityUtility.MathU;

public class Puppet : MonoBehaviour
{
    public PuppetSettings Settings => m_puppetSettings;

    [SerializeField] private PuppetSettings m_puppetSettings;
    [SerializeField] private PuppetBehaviour m_puppetBehaviour;

    private void Start()
    {
        m_puppetBehaviour.StartBehaviour(this);
    }

    private void Update()
    {
        m_puppetBehaviour.UpdateBehaviour(Time.deltaTime);
    }

    #region Puppet Movement
    public void MoveForward(float distance)
    {
        transform.position += transform.forward * distance;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
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
