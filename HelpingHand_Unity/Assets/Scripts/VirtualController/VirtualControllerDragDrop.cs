using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualControllerDragDrop : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private RectTransform m_rect;

    [SerializeField]
    private Texture2D m_cursorHover;

    [SerializeField]
    private Texture2D m_cursorMove;

    private bool m_isHovering;
    private bool m_isDragging;
    private Vector2 m_deltaPosition;
    private Vector2 m_halfDimension;

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 originPosition = eventData.position;
        m_deltaPosition = (Vector2) m_rect.position - originPosition;
        
        m_isDragging = true;
        Cursor.SetCursor(m_cursorMove, new Vector2(16, 16), CursorMode.Auto);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isDragging = false;
        Cursor.SetCursor(m_isHovering ? m_cursorHover : null, new Vector2(16, 16), CursorMode.Auto);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_halfDimension.x = m_rect.sizeDelta.x * m_rect.lossyScale.x / 2f;
        m_halfDimension.y = m_rect.sizeDelta.y * m_rect.lossyScale.y / 2f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 targetPos = eventData.position + m_deltaPosition;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        float clampedPosX = Mathf.Clamp(targetPos.x, 0 + m_halfDimension.x, screenSize.x - m_halfDimension.x);
        float clampedPosY = Mathf.Clamp(targetPos.y, 0 + m_halfDimension.y, screenSize.y - m_halfDimension.y);
        Vector2 clampedPos = new Vector2(clampedPosX, clampedPosY);
        m_rect.position = clampedPos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_isHovering = true;
        if (m_isDragging)
        {
            return;
        }
        Cursor.SetCursor(m_cursorHover, new Vector2(16, 16), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_isHovering = false;
        if (m_isDragging)
        {
            return;
        }
        Cursor.SetCursor(null, new Vector2(16, 16), CursorMode.Auto);
    }
}
