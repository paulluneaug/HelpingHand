using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISliderSounds : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Wwise Events")]
    [Tooltip("Looped sound while slider is moving")]
    public AK.Wwise.Event PlaySliderSfx;

    [Tooltip("Fade out when slider movement ends")]
    public AK.Wwise.Event StopSliderSfx_Fadout;

    [Tooltip("Quick fade-out when slider movement ends")]
    public AK.Wwise.Event StopSliderSfx_Immediate;

    [Tooltip("Triggered when slider reaches max")]
    public AK.Wwise.Event OnSliderMaxValue;

    [Tooltip("Triggered when slider reaches min")]
    public AK.Wwise.Event OnSliderMinValue;

    [Header("Slider Settings")]
    public Slider Slider;

    [Tooltip("RTPC to control the sound played in the blend track according to the slider value")]
    public AK.Wwise.RTPC RTPC_SliderValue;


    [Tooltip("RTPC to control pitch/speed of the scrub sound")]
    public AK.Wwise.RTPC RTPC_SliderSpeed;

    private float m_lastSliderValue = 0f;
    private float m_previousSliderValue = -1f;
    private float m_timeSinceLastChange = 0f;

    private uint m_playingId = AkUnitySoundEngine.AK_INVALID_PLAYING_ID;

    [Header("Timing Settings")]
    [Tooltip("Delay (in seconds) before stopping sound when slider is idle")]
    public float StopDelay = .2f;

    private bool m_hasTriggeredMax = false;
    private bool m_hasTriggeredMin = false;

    // ---    UI EVENT CALLBACKS   ---  //

    public void OnPointerDown(PointerEventData eventData)        // Start sound when we click on the button
    {
        m_playingId = PlaySliderSfx.Post(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)         // Stop sound when we release the button
    {
        _ = StopSliderSfx_Immediate.Post(gameObject);
    }

    private void Update()
    {
        float currentValue = Slider.value;
        RTPC_SliderValue.SetValue(gameObject, currentValue); // Slider value = RTPC value

        // Calculate and send speed to RTPC
        float deltaValue = Mathf.Abs(currentValue - m_lastSliderValue);
        float speed = deltaValue / Time.deltaTime;
        float normalizedSpeed = Mathf.Clamp(speed * 0.1f, 0f, 100f);
        RTPC_SliderSpeed.SetValue(gameObject, normalizedSpeed);
        m_lastSliderValue = currentValue;

        // Detect movement
        if (Mathf.Abs(currentValue - m_previousSliderValue) > 0.001f)
        {
            m_previousSliderValue = currentValue;
            m_timeSinceLastChange = 0f;

            if (m_playingId == AkUnitySoundEngine.AK_INVALID_PLAYING_ID)
            {
                m_playingId = PlaySliderSfx.Post(gameObject);
            }

            // Detect min value
            if (Mathf.Approximately(currentValue, MasterSlider.MIN_VALUE) && !m_hasTriggeredMin)
            {
                _ = (OnSliderMinValue?.Post(gameObject));
                _ = StopSliderSfx_Immediate.Post(gameObject);
                m_hasTriggeredMin = true;
            }
            else if (!Mathf.Approximately(currentValue, 0f))
            {
                m_hasTriggeredMin = false;
            }

            // Detect max value
            if (Mathf.Approximately(currentValue, MasterSlider.MAX_VALUE) && !m_hasTriggeredMax)
            {
                _ = (OnSliderMaxValue?.Post(gameObject));
                _ = StopSliderSfx_Immediate.Post(gameObject);
                m_hasTriggeredMax = true;
            }
            else if (!Mathf.Approximately(currentValue, 1f))
            {
                m_hasTriggeredMax = false;
            }
        }
        else
        {
            // No movement
            m_timeSinceLastChange += Time.deltaTime;

            if (m_timeSinceLastChange >= StopDelay && m_playingId != AkUnitySoundEngine.AK_INVALID_PLAYING_ID)
            {
                _ = StopSliderSfx_Fadout.Post(gameObject);
                m_playingId = AkUnitySoundEngine.AK_INVALID_PLAYING_ID;
            }
        }
    }
}
