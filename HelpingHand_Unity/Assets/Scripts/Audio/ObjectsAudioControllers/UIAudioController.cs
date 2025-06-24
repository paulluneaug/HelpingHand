using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable), typeof(RectTransform))]
public class UIAudioController : MonoBehaviour, IMoveHandler, ISubmitHandler
{
    public void OnMove(AxisEventData eventData)
    {
        AudioManager.Instance.PlayUINavigationSound();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        AudioManager.Instance.PlayUISubmitSound();
    }
}
