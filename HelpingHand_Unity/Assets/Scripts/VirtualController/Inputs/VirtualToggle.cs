using System;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.Easings;
using UnityUtility.Timer;

[RequireComponent(typeof(RectTransform))]
public class VirtualToggle : VirtualInput<bool>
{
    [Serializable]
    private class ToggleStateParameters
    {
        public Color BackgroundColor;
        public RectTransform TogglePosition;
    }

    [SerializeField] private Button m_button;
    [SerializeField] private Image m_background;
    [SerializeField] private RectTransform m_handle;

    [SerializeField] private ToggleStateParameters m_toggleDownParameters;
    [SerializeField] private ToggleStateParameters m_toggleUpParameters;

    [SerializeField] private float m_transitionTime;
    [SerializeField] private Easings.EasingFunction m_transitionEasing;

    [NonSerialized] private Timer m_transitionTimer;

    private void Awake()
    {
        SetValueWithoutNotify(false);

        m_button.onClick.AddListener(OnButtonClicked);
        ToggleStateParameters currentToggleParameters = GetToggleParameters(Value);

        m_handle.position = currentToggleParameters.TogglePosition.position;
        m_background.color = currentToggleParameters.BackgroundColor;

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
            ToggleStateParameters targetToggleParameters = GetToggleParameters(Value);
            m_handle.position = targetToggleParameters.TogglePosition.position;
            m_background.color = targetToggleParameters.BackgroundColor;
            return;
        }

        ToggleStateParameters currentToggleParameters = GetToggleParameters(Value);
        ToggleStateParameters otherToggleParameters = GetToggleParameters(!Value);

        float lerpFactor = Easings.Ease(m_transitionTimer.Progress, m_transitionEasing);
        m_handle.position = Vector3.Lerp(otherToggleParameters.TogglePosition.position, currentToggleParameters.TogglePosition.position, lerpFactor);
        m_background.color = Color.Lerp(otherToggleParameters.BackgroundColor, currentToggleParameters.BackgroundColor, lerpFactor);
    }

    private void OnDestroy()
    {
        m_button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnValidate()
    {
        if (m_background == null)
        {
            return;
        }

        m_background.color = m_toggleDownParameters.BackgroundColor;
    }

    public void SetToggleValue(bool value)
    {
        if (value == Value)
        {
            return;
        }

        OnButtonClicked();
    }

    private void OnButtonClicked()
    {
        SetValue(!Value);

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

    private ToggleStateParameters GetToggleParameters(bool value)
    {
        return value ? m_toggleUpParameters : m_toggleDownParameters;
    }

    protected override BaseVariable<bool> LinkedVariable { get; }
}
