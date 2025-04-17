using System;

using UnityEngine;

public class SingleSliderInteractiveObject : MonoBehaviour, IInteractiveObject
{
    public MasterSlider Slider => m_masterSlider;

    [SerializeField] private SlidersManager.SliderIndex m_controllingSlider;

    [NonSerialized] protected MasterSlider m_masterSlider;


    protected virtual void Start()
    {
        m_masterSlider = GameManager.Instance.SlidersManager.GetSlider(m_controllingSlider);
    }
}
