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
    public AK.Wwise.Event PlaySliderSfx;
    public AK.Wwise.Event StopSliderSfx;
    public AK.Wwise.Event OnSliderMaxValue;
    public AK.Wwise.Event OnSliderMinValue;

    public AK.Wwise.RTPC RTPC_Slider;
    public Slider Slider_1;
    float previousSliderValue = -1f;
    bool hasplayed=false;
    bool isSliderSoundPlaying = false;

    private uint playingId = AkSoundEngine.AK_INVALID_PLAYING_ID; //ID de lecture de l'évenement


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
        OnPointerUpSlider();
        if (Mathf.Abs(Slider_1.value - previousSliderValue) > 0.01f)
        {
            Debug.Log(Slider_1.value);
            RTPC_Slider.SetValue(null, Slider_1.value);

            if (!isSliderSoundPlaying)
            {
                isSliderSoundPlaying = true;
                playingId = PlaySliderSfx.Post(gameObject, (uint)AkCallbackType.AK_EndOfEvent, SliderSoundEnded);
            }

            previousSliderValue = Slider_1.value;
        }

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

    private void SliderSoundEnded(object in_cookie, AkCallbackType in_type, object in_info)
    {
        if (in_type == AkCallbackType.AK_EndOfEvent)
        {
            isSliderSoundPlaying = false;
        }
    }

    // Appelée quand la valeur du slider change
    public void TriggerSliderSeek()
    {
        if (playingId != AkSoundEngine.AK_INVALID_PLAYING_ID)
        {
            float sliderValue = Slider_1.value;
            float percentage = Mathf.InverseLerp(Slider_1.minValue, Slider_1.maxValue, sliderValue); // convert to 0.0 - 1.0

            // Seek to the corresponding position in the sound
            AkSoundEngine.SeekOnEvent(PlaySliderSfx.Id, gameObject, percentage, false);
        }
    }

    public void OnPointerUpSlider()
    {
        Debug.Log("Slider handle released.");
        StopSliderSfx.Post(gameObject);
        
    }


}



