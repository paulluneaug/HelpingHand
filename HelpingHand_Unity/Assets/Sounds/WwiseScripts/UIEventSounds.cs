    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIEventSounds : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public AK.Wwise.Event OnPointerDownSound;
        public AK.Wwise.Event OnPointerUpSound;
        public AK.Wwise.Event OnPointerEnterSound;
        public AK.Wwise.Event OnPointerExitSound;

        // Sert à jouer des sons comme le EventTrigger d'Unity : quand on survole un élément, quand on clique dessus, quand on relâche le clique et quand on sort de l'élément
        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDownSound.Post(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterSound.Post(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitSound.Post(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUpSound.Post(gameObject);
        }
    }
