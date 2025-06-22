using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventSounds : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Sert à jouer des sons comme le EventTrigger d'Unity : quand on survole un élément, quand on clique dessus, quand on relâche le clique et quand on sort de l'élément
    public void OnPointerDown(PointerEventData eventData)
    {
        _ = AudioManager.Instance.EventManager.ButtonOnPointerDown_Play.Post(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _ = AudioManager.Instance.EventManager.ButtonOnPointerEnter_Play.Post(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _ = AudioManager.Instance.EventManager.ButtonOnPointerExit_Play.Post(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _ = AudioManager.Instance.EventManager.ButtonOnPointerUp_Play.Post(gameObject);
    }
}
