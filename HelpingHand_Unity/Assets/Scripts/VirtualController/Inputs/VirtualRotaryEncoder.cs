using System;

using UnityEngine;
using UnityEngine.EventSystems;

using UnityUtility.Extensions;

[RequireComponent(typeof(RectTransform))]
public class VirtualRotaryEncoder : VirtualInput<int>, IPointerDownHandler, IDragHandler
{
    protected override BaseVariable<int> LinkedVariable => m_event.Index;

    [SerializeField] private RotaryEncoderInputEvent m_event;

    [SerializeField] private int m_stepCount;

    [SerializeField] private RectTransform m_knob;
    [SerializeField] private bool m_reverse;

    [NonSerialized] private float m_angle;

    [NonSerialized] private RectTransform m_rectTransform;
    [NonSerialized] private Vector2 m_dragLastPosition;
    [NonSerialized] private float m_dragLastAngle;

    [NonSerialized] private float m_step;
    [NonSerialized] private int m_index;


    protected void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_step = 360.0f / m_stepCount;

        m_index = 0;
        SetValueWithoutNotify(m_index);

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
        
        if (stepOffset == 0)
        {
            return;
        }

        if (stepOffset > 0)
        {
            m_event.RaiseStepRight();
        } 
        else
        {
            m_event.RaiseStepLeft();
        }

        
        m_index = (m_index + (m_reverse ? 1 : -1) * stepOffset).Mod(m_stepCount);
        SetValue(m_index);

        float snappedOffset = stepOffset * m_step;
        m_angle = m_dragLastAngle + snappedOffset;

        UpdateKnobPosition();

        m_dragLastPosition = currentLocalPosition;
        m_dragLastAngle = m_angle;
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
