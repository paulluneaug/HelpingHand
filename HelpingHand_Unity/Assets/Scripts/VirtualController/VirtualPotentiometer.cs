using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UnityUtility.Extensions;

[RequireComponent(typeof(RectTransform))]
public class VirtualPotentiometer : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
{
    #region Interface implementation
    public void GraphicUpdateComplete()
    {
    }

    public void LayoutComplete()
    {
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }

    public void Rebuild(CanvasUpdate executing)
    {
    }
    #endregion



    [SerializeField] private float m_originAngle;
    [SerializeField] private float m_range;

    [NonSerialized] private float m_value;

    [NonSerialized] private RectTransform m_rectTransform;
    [NonSerialized] private Vector2 m_dragStartPosition;

    protected override void Awake()
    {
        base.Awake();
        m_value = 0.0f;

        m_rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        m_dragStartPosition = ToLocalPosition(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentLocalPosition = ToLocalPosition(eventData.position);

        Vector2 offset = currentLocalPosition - m_dragStartPosition;


        

    }

    private Vector2 ToLocalPosition(Vector2 position)
    {
        return position - m_rectTransform.pivot;
    }
}
