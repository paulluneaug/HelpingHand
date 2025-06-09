using System;

using UnityEngine;

using UnityUtility.CustomAttributes;

using WwiseEvent = AK.Wwise.Event;
using WwiseRTPC = AK.Wwise.RTPC;


[Serializable]
public class SlidingObjectWwiseEventCollection
{
    [Title("Wwise Events")]
    [Tooltip("Looped sound while slider is moving")]
    public WwiseEvent PlaySliderSfx;

    [Tooltip("Fade out when slider movement ends")]
    public WwiseEvent StopSliderSfx_Fadout;

    [Tooltip("Quick fade-out when slider movement ends")]
    public WwiseEvent StopSliderSfx_Immediate;

    [Tooltip("Triggered when slider reaches max")]
    public WwiseEvent OnSliderMaxValue;

    [Tooltip("Triggered when slider reaches min")]
    public WwiseEvent OnSliderMinValue;

    [Title("RTPC")]
    [Tooltip("RTPC to control the sound played in the blend track according to the slider value")]
    public WwiseRTPC RTPC_SliderValue;

    [Tooltip("RTPC to control pitch/speed of the scrub sound")]
    public WwiseRTPC RTPC_SliderSpeed;
}
