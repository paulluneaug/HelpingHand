using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventBubbleUp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISelectHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        _ = ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _ = ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerUpHandler);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _ = ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerClickHandler);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _ = ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.selectHandler);
    }
}