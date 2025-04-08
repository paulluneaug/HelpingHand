using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEventSounds : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public AK.Wwise.Event OnPointerDownSound;
    public AK.Wwise.Event OnPointerUpSound;
    public AK.Wwise.Event OnPointerEnterSound;
    public AK.Wwise.Event OnPointerExitSound;
    public AK.Wwise.Event BlendTrack;

    public AK.Wwise.Event OnSliderMaxValue;
    public AK.Wwise.Event OnSliderMinValue;

    public AK.Wwise.RTPC RTPC_Slider;
    public Slider Slider_1;
    float previousSliderValue = -1f;
    bool hasplayed=false;

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


    public void TriggerSliderValueChange()
    {
        if (Mathf.Abs(Slider_1.value - previousSliderValue) > 0.01f) // small threshold to avoid float noise
        {
            Debug.Log(Slider_1.value);
            RTPC_Slider.SetValue(null, Slider_1.value);
            BlendTrack.Post(gameObject);

            previousSliderValue = Slider_1.value;

            if (Slider_1.value == Slider_1.maxValue)
            {
                OnSliderMaxValue.Post(gameObject);
                Debug.Log("MaxValue!");
            }
            else if (Slider_1.value == Slider_1.minValue)
            {
                OnSliderMinValue.Post(gameObject);
                Debug.Log("MinValue!");
            }
        }
    }


}


