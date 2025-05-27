using System;

using UnityEngine;

public class SingleSliderInteractiveObject : MonoBehaviour, ISlidingObject
{
    #region ISlidingObject Implementation
    public float SliderMaxValue => MasterSlider.MAX_VALUE;

    public float SliderMinValue => MasterSlider.MIN_VALUE;

    public float SliderValue => m_masterSlider.SliderValue;

    public float SliderSpeed => m_masterSlider.SliderSpeed;

    public float SliderMovementDirection => m_masterSlider.SliderMovementDirection;

    public event Action<float> OnSliderValueChanged;
    public event Action<bool> OnSliderPointerDown;
    #endregion

    public MasterSlider Slider => m_masterSlider;

    [SerializeField] private SlidersManager.SliderIndex m_controllingSlider;

    [NonSerialized] protected MasterSlider m_masterSlider;

    protected virtual void Start()
    {
        //m_masterSlider = GameManager.Instance.SlidersManager.GetSlider(m_controllingSlider);

        m_masterSlider.OnSliderValueChanged += OnSliderValueChanged_Callback;
        m_masterSlider.OnSliderPointerDown += OnSliderPointerDown_Callback;
    }

    protected virtual void OnDestroy()
    {
        m_masterSlider.OnSliderValueChanged -= OnSliderValueChanged_Callback;
        m_masterSlider.OnSliderPointerDown -= OnSliderPointerDown_Callback;
    }

    protected virtual void OnSliderValueChanged_Callback(float sliderValue)
    {
        OnSliderValueChanged?.Invoke(sliderValue);
    }

    protected virtual void OnSliderPointerDown_Callback(bool pointerDown)
    {
        OnSliderPointerDown?.Invoke(pointerDown);
    }
}
