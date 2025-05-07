using System;

using UnityEngine;
using UnityEngine.EventSystems;

using UnityUtility.Extensions;
using UnityUtility.MathU;

using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(RectTransform))]
public class VirtualRotaryEncoder : UIBehaviour, IPointerDownHandler, IDragHandler
{
    private enum RotationDirection
    {
        Clockwise,
        CounterClockwise,
    }

    [SerializeField] private float m_stepCount;

    [SerializeField] private RectTransform m_knob;

    [SerializeField] private int m_value;
    [NonSerialized] private float m_angle;

    [NonSerialized] private RectTransform m_rectTransform;
    [NonSerialized] private Vector2 m_dragLastPosition;
    [NonSerialized] private float m_dragLastAngle;

    [NonSerialized] private float m_step;

    protected override void Awake()
    {
        base.Awake();
        m_rectTransform = GetComponent<RectTransform>();
        m_step = 360.0f / m_stepCount;

        m_value = 0;

        UpdateKnobPosition();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_dragLastPosition = ToLocalPosition(eventData.position);
        m_dragLastAngle = m_angle;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentLocalPosition = ToLocalPosition(eventData.position);

        float angleOffset = Vector2.SignedAngle(m_dragLastPosition, currentLocalPosition);
        int stepOffset = (int)(angleOffset / m_step);

        if (stepOffset != 0) 
        {
            float snappedOffset = stepOffset * m_step;

            m_value += stepOffset;

            m_angle = m_dragLastAngle + snappedOffset;

            UpdateKnobPosition();

            m_dragLastPosition = currentLocalPosition;
            m_dragLastAngle = m_angle;
        }
    }

    private void UpdateKnobPosition()
    {
        m_knob.localRotation = Quaternion.AngleAxis(m_angle, m_knob.forward);
    }

    private Vector2 ToLocalPosition(Vector2 position)
    {
        return position - m_rectTransform.position.XY();
    }
}
