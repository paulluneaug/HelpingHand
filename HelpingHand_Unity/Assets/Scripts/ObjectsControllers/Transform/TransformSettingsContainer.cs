using System;

using UnityEngine;

[Serializable]
public class TransformSettingsContainer : IObjectSettingsContainer
{
    [SerializeField] private Transform m_transform;

    [SerializeField] private TransformSettings m_minSettings;
    [SerializeField] private TransformSettings m_maxSettings;


    public void Init()
    {
    }

    public void UpdateSettings(float progress)
    {
        if (m_transform == null)
        {
            return;
        }

        m_transform.localPosition = Vector3.Lerp(m_minSettings.LocalPosition, m_maxSettings.LocalPosition, progress);
        m_transform.localRotation = Quaternion.Slerp(m_minSettings.LocalRotation, m_maxSettings.LocalRotation, progress);
        m_transform.localScale = Vector3.Lerp(m_minSettings.LocalScale, m_maxSettings.LocalScale, progress);
    }

    public void Dispose()
    {
    }
}

[Serializable]
public class TransformSettings
{
    public Vector3 LocalPosition => m_localPosition;
    public Quaternion LocalRotation => m_localRotation;
    public Vector3 LocalScale => m_localScale;

    [SerializeField] private Vector3 m_localPosition = Vector3.zero;
    [SerializeField] private Quaternion m_localRotation = Quaternion.identity;
    [SerializeField] private Vector3 m_localScale = Vector3.one;
}
