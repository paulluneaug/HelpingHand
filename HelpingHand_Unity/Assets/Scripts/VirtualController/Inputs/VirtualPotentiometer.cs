using System;

using UnityEngine;
using UnityEngine.EventSystems;

using UnityUtility.Extensions;
using UnityUtility.MathU;

[RequireComponent(typeof(RectTransform))]
public class VirtualPotentiometer : VirtualInput<float>, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private enum RotationDirection
    {
        Clockwise,
        CounterClockwise,
    }


    [SerializeField] private float m_originAngle;
    [SerializeField] private float m_range;
    [SerializeField] private RotationDirection m_direction = RotationDirection.Clockwise;

    [SerializeField] private RectTransform m_knob;

    [NonSerialized] private float m_angle;

    [NonSerialized] private RectTransform m_rectTransform;
    [NonSerialized] private Vector2 m_dragLastPosition;
    [NonSerialized] private float m_dragLastAngle;

    [NonSerialized] private Vector2 m_knobRange;


    protected void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();

        m_knobRange = GetKnobRange();
        m_angle = m_knobRange.x;
        SetValueWithoutNotify(ComputeValue());

        UpdateKnobPosition();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_dragLastPosition = ToLocalPosition(eventData.position);
        m_dragLastAngle = m_angle;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentLocalPosition = ToLocalPosition(eventData.position);

        float angleOffset = Vector2.SignedAngle(m_dragLastPosition, currentLocalPosition);

        m_angle = MathUf.Clamp(m_dragLastAngle + angleOffset, MathUf.Min(m_knobRange.x, m_knobRange.y), MathUf.Max(m_knobRange.x, m_knobRange.y));

        UpdateKnobPosition();
        float newValue = ComputeValue();

        if (newValue != Value)
        {
            SetValue(newValue);
        }

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

    private Vector2 GetKnobRange()
    {
        return m_direction switch
        {
            RotationDirection.Clockwise => new Vector2(m_originAngle + m_range, m_originAngle),
            RotationDirection.CounterClockwise => new Vector2(m_originAngle, m_originAngle + m_range),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private float ComputeValue()
    {
        return m_angle.RemapTo01(m_knobRange);
    }

    private void OnDrawGizmos()
    {
        Vector2 knobRange = GetKnobRange() * MathUf.DEG_2_RAD;
        float rangeLineLength = ((RectTransform)transform).sizeDelta.x / 2.0f * transform.lossyScale.x;
        Vector3 position = transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(position, new Vector3(MathUf.Cos(knobRange.x) * rangeLineLength + position.x, MathUf.Sin(knobRange.x) * rangeLineLength + position.y, position.z));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(position, new Vector3(MathUf.Cos(knobRange.y) * rangeLineLength + position.x, MathUf.Sin(knobRange.y) * rangeLineLength + position.y, position.z));
    }

    protected override BaseVariable<float> LinkedVariable { get; }
}
