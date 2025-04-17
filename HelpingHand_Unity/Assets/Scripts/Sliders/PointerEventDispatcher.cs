using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventDispatcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action<bool> OnPointerDownChanged;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownChanged?.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerDownChanged?.Invoke(false);
    }
}
