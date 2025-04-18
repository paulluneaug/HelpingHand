using UnityEngine;
using UnityEngine.EventSystems;


using WwiseEvent = AK.Wwise.Event;

public class UIEventSounds : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private WwiseEvent m_onPointerDownSound;
    [SerializeField] private WwiseEvent m_onPointerUpSound;
    [SerializeField] private WwiseEvent m_onPointerEnterSound;
    [SerializeField] private WwiseEvent m_onPointerExitSound;

    // Sert à jouer des sons comme le EventTrigger d'Unity : quand on survole un élément, quand on clique dessus, quand on relâche le clique et quand on sort de l'élément
    public void OnPointerDown(PointerEventData eventData)
    {
        _ = m_onPointerDownSound?.Post(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _ = m_onPointerEnterSound?.Post(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _ = m_onPointerExitSound?.Post(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _ = m_onPointerUpSound?.Post(gameObject);
    }
}
