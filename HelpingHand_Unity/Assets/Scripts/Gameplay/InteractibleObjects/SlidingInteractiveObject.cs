using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.MathU;

public class SlidingInteractiveObject : SingleSliderInteractiveObject
{
    [Title("Component References")]
    [SerializeField] private Transform m_start;
    [SerializeField] private Transform m_end;

    [SerializeField] private Rigidbody m_physics;
    [SerializeField] private Transform m_model;

    [Title("Sliding Settings")]
    [SerializeField] private float m_halfLife;

    private void Update()
    {
        m_model.position = MathUf.SmoothLerp(m_model.position, GetTargetPosition(), Time.deltaTime, m_halfLife);
    }

    private void FixedUpdate()
    {
        m_physics.MovePosition(MathUf.SmoothLerp(m_physics.position, GetTargetPosition(), Time.fixedDeltaTime, m_halfLife));
    }

    private Vector3 GetTargetPosition()
    {
        return Vector3.Lerp(m_start.position, m_end.position, m_masterSlider.SliderValue);
    }

}
