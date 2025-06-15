using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.MathU;

[Serializable]
public class LightSettingsContainer : IObjectSettingsContainer
{
    [SerializeField] private Light m_spot;

    [SerializeField] private Gradient m_colorGradient;
    [SerializeField] private LightSettings m_minSettings;
    [SerializeField] private LightSettings m_maxSettings;


    public void Init()
    {
    }

    public void UpdateSettings(float progress)
    {
        if (m_spot == null)
        {
            return;
        }

        Color color = m_colorGradient.Evaluate(progress);
        float intensity = MathUf.Lerp(m_minSettings.Intensity, m_maxSettings.Intensity, progress);
        float range = MathUf.Lerp(m_minSettings.Range, m_maxSettings.Range, progress);
        Vector2 angles = Vector2.Lerp(m_minSettings.SpotAngles, m_maxSettings.SpotAngles, progress);

        m_spot.color = color;
        m_spot.intensity = intensity;
        m_spot.range = range;
        m_spot.innerSpotAngle = angles.x;
        m_spot.spotAngle = angles.y;
    }

    public void Dispose()
    {
    }
}

[Serializable]
public class LightSettings
{
    public float Intensity => m_intensity;
    public float Range => m_range;
    public Vector2 SpotAngles  => m_spotAngles;

    [SerializeField, Min(0.0f)] private float m_intensity;
    [SerializeField, Min(0.0f)] private float m_range;
    [SerializeField, MinMaxSlider(0.0f, 179.0f)] private Vector2 m_spotAngles;
}
