using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonEventInvoker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private ButtonInputEvent m_buttonEvent;

    private bool m_isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_buttonEvent.RaiseDown();
        m_isPressed = true;
        StartCoroutine(ButtonPressedCoroutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_buttonEvent.RaiseUp();
        m_isPressed = false;
    }

    private IEnumerator ButtonPressedCoroutine()
    {
        while (m_isPressed)
        {
            m_buttonEvent.RaisePressed();
            yield return null;
        }
    }
}
