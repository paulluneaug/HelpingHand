using System;

using UnityEngine;

public class PotentiometerEventRaiser : MonoBehaviour
{
    [SerializeField] private BaseVariable<float> m_faderVariable;

    [SerializeField] private AK.Wwise.Event m_minValueEvent;
    [SerializeField] private AK.Wwise.Event m_maxValueEvent;
    [SerializeField] private AK.Wwise.RTPC m_valueRTPC;
    [SerializeField] private AK.Wwise.RTPC m_speedRTPC;

    public void Start()
    {
        m_valueRTPC.SetValue(gameObject, 0.0f);
    }
}
