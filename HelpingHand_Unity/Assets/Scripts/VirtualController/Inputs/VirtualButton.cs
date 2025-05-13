using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UnityUtility.Easings;
using UnityUtility.Timer;

public class VirtualButton : VirtualInput<bool>, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private Color m_buttonReleasedColor;
    [SerializeField] private Color m_buttonPressedColor;
    [SerializeField] private Image m_background;

    [SerializeField] private float m_transitionTime;
    [SerializeField] private Easings.EasingFunction m_transitionEasing;

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
            m_background.color = GetButtonColor(Value);
            return;
        }

        Color currentButtonColor = GetButtonColor(Value);
        Color otherButtonColor = GetButtonColor(!Value);

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
        OnButtonPressed(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnButtonPressed(false);
    }

    private Color GetButtonColor(bool state)
    {
        return state ? m_buttonPressedColor : m_buttonReleasedColor;
    }

    private void OnButtonPressed(bool buttonValue)
    {
        SetValue(buttonValue);

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

    protected override BaseVariable<bool> LinkedVariable { get; }
}
