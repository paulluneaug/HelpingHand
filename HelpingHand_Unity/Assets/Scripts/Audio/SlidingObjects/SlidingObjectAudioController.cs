using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;
using UnityUtility.MathU;
using UnityUtility.Timer;

using WwiseEvent = AK.Wwise.Event;

[RequireComponent(typeof(ISlidingObject))]
public class SlidingObjectAudioController : MonoBehaviour
{
    [SerializeField] private SlidingObjectWwiseEventCollection m_wwiseEvents;
    [Separator]

    [SerializeField] private float m_valueTolerance = 0.05f;

    [Tooltip("Delay (in seconds) before stopping sound when slider is idle")]
    [SerializeField] private float m_fadeoutDelay = 0.2f;

    // Cache
    [NonSerialized] private ISlidingObject m_slidingObject;

    [NonSerialized] private uint m_playingEventID;
    [NonSerialized] private bool m_tiggeredMinValueEvent;
    [NonSerialized] private bool m_tiggeredMaxValueEvent;

    [NonSerialized] private Timer m_fadeoutTimer;

    private void Start()
    {
        m_slidingObject = GetComponent<ISlidingObject>();
        m_slidingObject.OnSliderPointerDown += OnSliderPointerDown;
        m_slidingObject.OnSliderValueChanged += OnSliderValueChanged;

        m_tiggeredMinValueEvent = true;
        m_tiggeredMaxValueEvent = true;

        m_fadeoutTimer = new Timer(m_fadeoutDelay, false);
        m_fadeoutTimer.Start();
    }

    private void Update()
    {
        m_wwiseEvents.RTPC_SliderValue.SetValue(gameObject, m_slidingObject.SliderValue);
        m_wwiseEvents.RTPC_SliderSpeed.SetValue(gameObject, m_slidingObject.SliderSpeed);

        if (m_fadeoutTimer.Update(Time.deltaTime) && m_playingEventID != AkUnitySoundEngine.AK_INVALID_PLAYING_ID)
        {
            _ = m_wwiseEvents.StopSliderSfx_Fadout.Post(gameObject);
            m_playingEventID = AkUnitySoundEngine.AK_INVALID_PLAYING_ID;
        }
    }

    private void OnDestroy()
    {
        m_slidingObject.OnSliderPointerDown -= OnSliderPointerDown;
        m_slidingObject.OnSliderValueChanged -= OnSliderValueChanged;
    }

    private void OnSliderValueChanged(float sliderValue)
    {
        if (m_playingEventID == AkUnitySoundEngine.AK_INVALID_PLAYING_ID)
        {
            m_playingEventID = m_wwiseEvents.PlaySliderSfx.Post(gameObject);
        }

        // Slider at the min value
        _ = TriggerSliderValueEvents(
            sliderValue,
            m_slidingObject.SliderMinValue,
            ref m_tiggeredMinValueEvent,
            // Events to trigger
            m_wwiseEvents.OnSliderMinValue,
            m_wwiseEvents.StopSliderSfx_Immediate);

        // Slider at the max value
        _ = TriggerSliderValueEvents(
            sliderValue,
            m_slidingObject.SliderMaxValue,
            ref m_tiggeredMaxValueEvent,
            // Events to trigger
            m_wwiseEvents.OnSliderMaxValue,
            m_wwiseEvents.StopSliderSfx_Immediate);



        m_fadeoutTimer.Reset();
        m_fadeoutTimer.Start();
    }

    private void OnSliderPointerDown(bool pointerDown)
    {
        if (pointerDown)
        {
            m_playingEventID = m_wwiseEvents.PlaySliderSfx.Post(gameObject);
            m_fadeoutTimer.Reset();
            m_fadeoutTimer.Start();
        }
        else
        {
            _ = m_wwiseEvents.StopSliderSfx_Immediate.Post(gameObject);
        }
    }

    private bool TriggerSliderValueEvents(float sliderValue, float targetValue, ref bool alreadyTriggered, params WwiseEvent[] eventsToTrigger)
    {
        bool targetReached = MathUf.AbsoluteDifference(sliderValue, targetValue) < m_valueTolerance;
        if (targetReached)
        {
            if (!alreadyTriggered)
            {
                eventsToTrigger.ForEach(wwiseEvent => wwiseEvent?.Post(gameObject));
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

}
