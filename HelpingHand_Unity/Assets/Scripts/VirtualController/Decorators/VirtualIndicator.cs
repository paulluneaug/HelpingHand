using System;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

public class VirtualIndicator : MonoBehaviour
{
    public bool Enabled => m_enabled;

    [SerializeField] [Required] private IndicatorState m_state;
    [SerializeField] private Image m_image;
    [SerializeField] private Image m_glow;

    [SerializeField] private Color m_litColor;
    [SerializeField] private Color m_unlitColor;

    [NonSerialized] private bool m_enabled;

    private void Awake()
    {
        SetEnable(false);
    }

    private void OnEnable()
    {
        m_state.RemoveListener(OnStateChanged);
        m_state.AddListener(OnStateChanged);
    }

    private void OnDisable()
    {
        m_state.RemoveListener(OnStateChanged);
    }

    private void OnStateChanged(bool isSet)
    {
        SetEnable(isSet);
    }

    public void SetEnable(bool isEnabled)
    {
        m_enabled = isEnabled;
        m_glow.enabled = m_enabled;
        m_image.color = isEnabled ? m_litColor : m_unlitColor;
        m_glow.color = isEnabled ? m_litColor : m_unlitColor;
    }
}
