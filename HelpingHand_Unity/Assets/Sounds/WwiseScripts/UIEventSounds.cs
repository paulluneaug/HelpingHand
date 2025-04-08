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
        public AK.Wwise.Event SliderScrubEvent;
        public AK.Wwise.RTPC RTPC_SliderSpeed; // <-- NEW RTPC for pitch control
        private float lastSliderValue = 0f;
        private float sliderSpeed = 0f;

        private uint playingId = AkSoundEngine.AK_INVALID_PLAYING_ID;
        int lastStep = -1;
        private float timeSinceLastChange = 0f;
        public float stopDelay = 0.2f; // seconds of "no movement" before stopping
        private float previousSliderValue = -1f;

        public void OnPointerDown(PointerEventData eventData)
        {
            //OnPointerDownSound.Post(gameObject);
            playingId = SliderScrubEvent.Post(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //OnPointerEnterSound.Post(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //OnPointerExitSound.Post(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        // OnPointerUpSound.Post(gameObject);
            AkSoundEngine.ExecuteActionOnEvent(SliderScrubEvent.Id, AkActionOnEventType.AkActionOnEventType_Stop, gameObject); 
        }
    void Update()
    {
        float currentValue = Slider_1.value;
        float deltaValue = Mathf.Abs(currentValue - lastSliderValue);
        float speed = deltaValue / Time.deltaTime; // Units per second

        // Normalize the speed to be between 0 and 100
        float normalizedSpeed = Mathf.Clamp(speed * 0.1f, 0f, 100f);  // Scaling factor can be adjusted

        // Send to Wwise RTPC (the pitch control)
        RTPC_SliderSpeed.SetValue(gameObject, normalizedSpeed);
        sliderSpeed = speed;
        lastSliderValue = currentValue;

        // Debug log to check the normalized speed
        Debug.Log("Slider Speed: " + normalizedSpeed);

        // Existing movement detection logic
        if (Mathf.Abs(currentValue - previousSliderValue) > 0.001f)
        {
            previousSliderValue = currentValue;
            timeSinceLastChange = 0f;

            if (playingId == AkSoundEngine.AK_INVALID_PLAYING_ID)
            {
                playingId = SliderScrubEvent.Post(gameObject);
                Debug.Log("Sound restarted due to slider movement.");
            }
        }
        else
        {
            timeSinceLastChange += Time.deltaTime;

            if (timeSinceLastChange >= stopDelay && playingId != AkSoundEngine.AK_INVALID_PLAYING_ID)
            {
                Debug.Log("Slider stopped moving — stopping sound");
                AkSoundEngine.ExecuteActionOnEvent(SliderScrubEvent.Id, AkActionOnEventType.AkActionOnEventType_Stop, gameObject);
                playingId = AkSoundEngine.AK_INVALID_PLAYING_ID;
            }
        }

        // Step logic (optional)
        int currentStep = Mathf.FloorToInt(currentValue * 10);
        if (currentStep != lastStep)
        {
            //Debug.Log("Entered new step: " + currentStep);
            lastStep = currentStep;
        }

        RTPC_Slider.SetValue(gameObject, currentValue);
    }

    }
