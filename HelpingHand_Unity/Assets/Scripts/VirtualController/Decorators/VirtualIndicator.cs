using System;

using UnityEngine;
using UnityEngine.UI;

public class VirtualIndicator : MonoBehaviour
{
    public bool Enabled => m_enabled;

    [SerializeField] private Image m_image;
    [SerializeField] private Image m_glow;

    [SerializeField] private Color m_litColor;
    [SerializeField] private Color m_unlitColor;

    [NonSerialized] private bool m_enabled;

    private void Awake()
    {
        SetEnable(false);
    }

    public void SetEnable(bool enabled)
    {
        m_enabled = enabled;
        m_glow.enabled = m_enabled;
        m_image.color = enabled ? m_litColor : m_unlitColor;
        m_glow.color = enabled ? m_litColor : m_unlitColor;
    }
}
