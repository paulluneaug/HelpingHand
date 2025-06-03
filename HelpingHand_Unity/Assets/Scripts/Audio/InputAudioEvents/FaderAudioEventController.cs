using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;
using UnityUtility.Filters;
using UnityUtility.MathU;
using UnityUtility.Timer;

public class FaderAudioEventController : MonoBehaviour
{
    [SerializeField] private FloatInputEvent m_faderVariable;

    [Title("Wwise references")]
    [SerializeField] private WwiseInputEventPair m_loopEvent;
    [SerializeField] private WwiseInputEventPair m_minValueEvent;
    [SerializeField] private WwiseInputEventPair m_maxValueEvent;

    [SerializeField] private WwiseInputEventPair m_stopFadoutEvent;
    [SerializeField] private WwiseInputEventPair m_stopImmediateEvent;

    [SerializeField] private AK.Wwise.RTPC m_valueRTPC;
    [SerializeField] private AK.Wwise.RTPC m_speedRTPC;

    [Title("Parameters")]
    [SerializeField] private float m_valueTolerance = 0.05f;

    [Tooltip("Delay (in seconds) before stopping sound when slider is idle")]
    [SerializeField] private float m_fadeoutDelay = 0.2f;

    [SerializeField] private OneEuroFilterSettings m_speedFilterSettings;

    // Cache
    [NonSerialized] private bool m_init = false;

    [NonSerialized] private uint m_playingEventID;
    [NonSerialized] private bool m_tiggeredMinValueEvent;
    [NonSerialized] private bool m_tiggeredMaxValueEvent;

    [NonSerialized] private Timer m_fadeoutTimer;
    [NonSerialized] private OneEuroFilter m_speedFilter;

    [NonSerialized] private float m_speed;
    [NonSerialized] private float m_previousValue;


    public void Init()
    {
        m_faderVariable.OnEventRaised += OnSliderValueChanged;

        m_tiggeredMinValueEvent = true;
        m_tiggeredMaxValueEvent = true;

        m_fadeoutTimer = new Timer(m_fadeoutDelay, false);
        m_fadeoutTimer.Start();

        m_speedFilter = new OneEuroFilter(m_speedFilterSettings.MinCutoff, m_speedFilterSettings.Beta);
        m_speed = m_speedFilter.Filter(0.0f, 0.0f);

        m_init = true;
    }

    private void Update()
    {
        if (!m_init)
        {
            return;
        }

        float speed = UpdateFaderSpeed(Time.deltaTime);
        m_valueRTPC.SetValue(gameObject, m_faderVariable.Value);
        m_speedRTPC.SetValue(gameObject, speed);

        if (m_fadeoutTimer.Update(Time.deltaTime) && m_playingEventID != AkUnitySoundEngine.AK_INVALID_PLAYING_ID)
        {
            _ = m_stopFadoutEvent.PostEvent(m_faderVariable.IsActive, gameObject);
            m_playingEventID = AkUnitySoundEngine.AK_INVALID_PLAYING_ID;
        }
    }

    public void Dispose()
    {
        m_faderVariable.OnEventRaised -= OnSliderValueChanged;
        m_init = false;
    }

    private void OnSliderValueChanged()
    {
        float sliderValue = m_faderVariable.Value;
        Debug.Log($"Slider value changed : {sliderValue}");

        if (m_playingEventID == AkUnitySoundEngine.AK_INVALID_PLAYING_ID)
        {
            m_playingEventID = m_loopEvent.PostEvent(m_faderVariable.IsActive, gameObject);
        }

        // Slider at the min value
        _ = TriggerSliderValueEvents(
            sliderValue,
            0.0f,
            ref m_tiggeredMinValueEvent,
            // Events to trigger
            m_minValueEvent,
            m_stopImmediateEvent);

        // Slider at the max value
        _ = TriggerSliderValueEvents(
            sliderValue,
            1.0f,
            ref m_tiggeredMaxValueEvent,
            // Events to trigger
            m_maxValueEvent,
            m_stopImmediateEvent);



        m_fadeoutTimer.Reset();
        m_fadeoutTimer.Start();
    }

    private bool TriggerSliderValueEvents(float sliderValue, float targetValue, ref bool alreadyTriggered, params WwiseInputEventPair[] eventsToTrigger)
    {
        bool targetReached = MathUf.AbsoluteDifference(sliderValue, targetValue) < m_valueTolerance;
        if (targetReached)
        {
            if (!alreadyTriggered)
            {
                eventsToTrigger.ForEach(wwiseEvent => wwiseEvent.PostEvent(m_faderVariable.IsActive, gameObject));
                alreadyTriggered = true;
                return true;
            }
        }
        else
        {
            alreadyTriggered = false;
        }
        return false;
    }

    private float UpdateFaderSpeed(float deltaTime)
    {
        if (deltaTime <= 0.0f)
        {
            return m_speed;
        }

        float currentValue = m_faderVariable.Value;
        float deltaPosition = currentValue - m_previousValue;
        float speed = deltaPosition / deltaTime;

        float newSpeed = MathUf.Round(m_speedFilter.Filter(speed, deltaTime), 3);
        m_speed = MathUf.Abs(newSpeed);

        m_previousValue = currentValue;

        return m_speed;
    }
}
