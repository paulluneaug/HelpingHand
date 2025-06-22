using System;

using UnityEngine;

[Serializable]
public class SlidersManager
{
    public enum SliderIndex
    {
        Slider1,
        Slider2,
        Slider3,
        Slider4,
    }

    [SerializeField] private MasterSlider m_masterSlider1;
    [SerializeField] private MasterSlider m_masterSlider2;
    [SerializeField] private MasterSlider m_masterSlider3;
    [SerializeField] private MasterSlider m_masterSlider4;

    public MasterSlider GetSlider(SliderIndex sliderIndex)
    {
        return sliderIndex switch
        {
            SliderIndex.Slider1 => m_masterSlider1,
            SliderIndex.Slider2 => m_masterSlider2,
            SliderIndex.Slider3 => m_masterSlider3,
            SliderIndex.Slider4 => m_masterSlider4,
            _ => throw new ArgumentOutOfRangeException(nameof(sliderIndex), sliderIndex, ""),
        };
    }
}
