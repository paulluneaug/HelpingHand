using System;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

using UnityUtility.CustomAttributes;
using UnityUtility.Easings;
using UnityUtility.Timer;

[RequireComponent(typeof(RectTransform))]
public class MenuSelectableButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Serializable]
    private class State
    {
        public RectTransform Position;
        public Color TextColor;
    }

    [SerializeField] private State m_selectedState;
    [SerializeField] private State m_deselectedState;

    [SerializeField] private TMP_Text m_buttonText;

    [SerializeField] private float m_cogRotation;
    [SerializeField] private MenuCogController m_cogController;

    [Title("Transition")]
    [SerializeField] private float m_transitionTime;
    [SerializeField] private Easings.EasingFunction m_easingFunction = Easings.EasingFunction.Linear;

    [NonSerialized] private RectTransform m_rectTransform;
    [NonSerialized] private Timer m_transitionTimer;
    [NonSerialized] private bool m_selected = false;

    private void Awake()
    {
        m_transitionTimer = new Timer(m_transitionTime, false);
        m_rectTransform = (RectTransform)transform;
    }

    private void Update()
    {
        if (!m_transitionTimer.IsRunning)
        {
            return;
        }

        bool finished = m_transitionTimer.Update(Time.deltaTime);
        if (finished)
        {
            m_transitionTimer.Stop();
        }
        float progress = finished ? 1.0f : m_transitionTimer.Progress;

        if (!m_selected)
        {
            progress = 1.0f - progress;
        }

        float lerpFactor = Easings.Ease(progress, m_easingFunction);
        m_rectTransform.anchoredPosition = Vector2.Lerp(m_deselectedState.Position.anchoredPosition, m_selectedState.Position.anchoredPosition, lerpFactor);
        m_buttonText.color = Color.Lerp(m_deselectedState.TextColor, m_selectedState.TextColor, lerpFactor);

        if (m_selected)
        {
            m_cogController.UpdateTransition(lerpFactor);
        }

    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelectionChanged(true);
        m_cogController.SetTarget(m_cogRotation);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        OnSelectionChanged(false);
    }

    private void OnSelectionChanged(bool selectedState)
    {
        m_selected = selectedState;
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
