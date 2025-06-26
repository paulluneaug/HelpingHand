using System;

using UnityEngine;
using UnityEngine.Serialization;

using UnityUtility.CustomAttributes;
using UnityUtility.MathU;

public class SpotlightController : MonoBehaviour
{
    public enum SpotlightMode
    {
        FollowTarget,
        Manual,
    }

    [Title("Variable References")]
    [SerializeField] private BaseVariable<Vector2> m_manualInput;
    [SerializeField] private BaseVariable<bool> m_targetInCone;
    [SerializeField] private BaseVariable<Transform> m_target;
    [FormerlySerializedAs("m_autoMode")] [FormerlySerializedAs("m_manualMode")] [SerializeField] private BaseVariable<bool> m_followMode;
    [SerializeField] private ButtonInputEvent m_toggleSpotlightButton;
    [FormerlySerializedAs("m_toggleAutoModeIndicator")] [FormerlySerializedAs("m_toggleManualModeIndicator")] [SerializeField] private EntityState m_toggleFollowModeIndicator;

    [Title("Component References")]
    [SerializeField] private Transform m_spotTransform;
    [SerializeField] private Light m_light;

    [Title("Movement Settings")]
    [SerializeField, Tooltip("Degrees / sec")] private float m_manualRotationSpeed;
    [SerializeField, Tooltip("Degrees / sec")] private float m_followTargetRotationSpeed;

    [SerializeField, Tooltip("Degrees")] private float m_spotMaxRange;
    [SerializeField, Tooltip("Degrees")] private float m_spotAngle;

    [Title("Debug")]
    [SerializeField] private float m_coneHeight;

    // Cache
    private Transform m_transform;

    private SpotlightMode m_mode;


    private void Start()
    {
        m_transform = transform;
        m_toggleSpotlightButton.AddListener(OnSpotlightToggle);
        m_toggleFollowModeIndicator.AddListener(OnFollowModeToggle);
        m_mode = m_followMode.Value ? SpotlightMode.FollowTarget : SpotlightMode.Manual;
        m_light.enabled = false;
    }

    private void OnFollowModeToggle(bool isOn)
    {
        m_mode = isOn ? SpotlightMode.FollowTarget : SpotlightMode.Manual;
        m_followMode.Value = isOn;
    }

    private void OnDestroy()
    {
        m_toggleSpotlightButton.RemoveListener(OnSpotlightToggle);
        m_toggleFollowModeIndicator.RemoveListener(OnFollowModeToggle);
    }

    private void OnSpotlightToggle()
    {
        m_light.enabled = !m_light.enabled;
    }

    private void Update()
    {
        switch (m_mode)
        {
            case SpotlightMode.Manual:
                UpdateManualMovement();
                break;

            case SpotlightMode.FollowTarget:
                UpdateFollowTargetMovement();
                break;

            default:
                break;
        }

        m_targetInCone.Value = IsTargetInCone();
    }

    private void UpdateManualMovement()
    {
        if (!m_manualInput.IsActive)
        {
            return;
        }

        Vector2 input = m_manualInput.Value.normalized;
        float angle = m_manualRotationSpeed * Time.deltaTime;

        Vector3 spotLocalRotation = m_spotTransform.localRotation.eulerAngles;
        Vector3 previousRotation = spotLocalRotation;

        spotLocalRotation.x += -input.y * angle;
        spotLocalRotation.y += input.x * angle;

        m_spotTransform.localRotation = Quaternion.Euler(spotLocalRotation);
        if (Vector3.Angle(m_spotTransform.forward, m_transform.forward) > m_spotMaxRange)
        {
            m_spotTransform.localRotation = Quaternion.Euler(previousRotation);
        }
    }

    private void UpdateFollowTargetMovement()
    {
        if (m_target == null || m_target.Value == null)
        {
            Debug.LogWarning($"No target assigned");
            return;
        }
        float angle = m_followTargetRotationSpeed * Time.deltaTime;
        m_spotTransform.rotation = Quaternion.RotateTowards(m_spotTransform.rotation, Quaternion.LookRotation(m_target.Value.position - m_spotTransform.position), angle);
    }

    private bool IsTargetInCone()
    {
        if (m_target == null || m_target.Value == null)
        {
            Debug.LogWarning($"No target assigned");
            return false;
        }

        Vector3 toTarget = m_target.Value.position - m_spotTransform.position;
        float angleToTarget = Vector3.Angle(m_spotTransform.forward, toTarget);
        return angleToTarget < m_spotAngle;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmosExtensions.DrawConeFromAngle(transform.position, transform.forward, m_coneHeight, m_spotMaxRange * MathUf.DEG_2_RAD);

        if (m_spotTransform == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        GizmosExtensions.DrawConeFromAngle(m_spotTransform.position, m_spotTransform.forward, m_coneHeight, m_spotAngle * MathUf.DEG_2_RAD);
    }
}
