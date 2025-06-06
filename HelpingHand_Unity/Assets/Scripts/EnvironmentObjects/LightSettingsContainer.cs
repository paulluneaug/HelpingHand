using System;

using UnityEngine;

using UnityUtility.CustomAttributes;

[Serializable]
public class LightSettingsContainer
{
    [SerializeField] private Light m_spot;

    [SerializeField] private Gradient m_colorGradient;
    [SerializeField] private Range<Vector2> m_spotAngles;
    [SerializeField] private Range<float> m_intensity;
    [SerializeField] private Range<float> m_range;

    public void UpdateLightSettings(float progress)
    {
    }
}

[Serializable]
public class Range<T>
{
    public T Min => m_min;
    public T Max => m_max;

    [SerializeField] private T m_min;
    [SerializeField] private T m_max;
}
