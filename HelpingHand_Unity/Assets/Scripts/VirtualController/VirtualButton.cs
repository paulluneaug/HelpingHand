using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UnityUtility.Easings;
using UnityUtility.Timer;

public class VirtualButton : VirtualInput<bool>, IPointerDownHandler, IPointerUpHandler
{
    public override bool Value => m_value;
    public override event Action<bool> OnValueChanged;

    [SerializeField] private Color m_buttonReleasedColor;
    [SerializeField] private Color m_buttonPressedColor;
    [SerializeField] private Image m_background;

    [SerializeField] private float m_transitionTime;
    [SerializeField] private Easings.EasingFunction m_transitionEasing;

    [NonSerialized] private bool m_value;
    [NonSerialized] private Timer m_transitionTimer;

    private void Awake()
    {
        m_transitionTimer = new Timer(m_transitionTime, false);
    }

    private void Update()
    {
        if (!m_transitionTimer.IsRunning)
        {
            return;
        }
        if (m_transitionTimer.Update(Time.deltaTime))
        {
            m_transitionTimer.Stop();
            m_background.color = GetButtonColor(m_value);
            return;
        }

        Color currentButtonColor = GetButtonColor(m_value);
        Color otherButtonColor = GetButtonColor(!m_value);

        float lerpFactor = Easings.Ease(m_transitionTimer.Progress, m_transitionEasing);
        m_background.color = Color.Lerp(otherButtonColor, currentButtonColor, lerpFactor);
    }

    private void OnValidate()
    {
        if (m_background == null)
        {
            return;
        }

        m_background.color = m_buttonReleasedColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_value = true;
        OnButtonPressed();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_value = false;
        OnButtonPressed();
    }

    private Color GetButtonColor(bool state)
    {
        return state ? m_buttonPressedColor : m_buttonReleasedColor;
    }

    private void OnButtonPressed()
    {
        OnValueChanged?.Invoke(m_value);

        if (m_transitionTimer.IsRunning)
        {
            float timeToTarget = (1.0f - m_transitionTimer.Progress) * m_transitionTimer.Duration;
            m_transitionTimer.Reset();
            _ = m_transitionTimer.Update(timeToTarget);
        }
        else
        {
            m_transitionTimer.Reset();
        }

        m_transitionTimer.Start();
    }
}
