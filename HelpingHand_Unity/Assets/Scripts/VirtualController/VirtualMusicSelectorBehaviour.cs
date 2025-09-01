using System;

using Sirenix.OdinInspector;

using UnityEngine;

public class VirtualMusicSelectorBehaviour : MonoBehaviour
{
    [SerializeField] private VirtualRotaryEncoder m_musicRotary;

    [SerializeField] private ToggleInputEvent m_musicSelection0;
    [SerializeField] private ToggleInputEvent m_musicSelection1;

    [SerializeField, ReadOnly] private int m_selectedMusic = 0; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_musicRotary.OnValueChanged += OnRotaryValueChanged;
        m_selectedMusic = 0;
    }

    private void OnDestroy()
    {
        m_musicRotary.OnValueChanged -= OnRotaryValueChanged;
    }

    private void OnRotaryValueChanged(int rotaryPosition)
    {
        m_selectedMusic = rotaryPosition % 4;

        m_musicSelection0.Value = (m_selectedMusic & (1 << 0)) != 0;
        m_musicSelection1.Value = (m_selectedMusic & (1 << 1)) != 0;

    }
}
