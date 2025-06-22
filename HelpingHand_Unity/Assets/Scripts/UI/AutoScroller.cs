using System;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.Easings;
using UnityUtility.Timer;

[RequireComponent(typeof(ScrollRect))]
public class AutoScroller : MonoBehaviour
{
    [SerializeField] private float m_transitionTime;
    [SerializeField] private Easings.EasingFunction m_easingFunction;

    [NonSerialized] private ScrollRect m_scrollRect;
    [NonSerialized] private Timer m_transitionTimer;

    [NonSerialized] private Vector2 m_startPosition;
    [NonSerialized] private Vector2 m_targetPosition;

    private void Awake()
    {
        m_scrollRect = GetComponent<ScrollRect>();
        m_transitionTimer = new Timer(m_transitionTime, false);
    }

    private void Update()
    {
        if (!m_transitionTimer.IsRunning)
        {
            return;
        }

        float progress;
        if (m_transitionTimer.Update(Time.deltaTime))
        {
            progress = 1.0f;
            m_transitionTimer.Stop();
        }
        else
        {
            progress = m_transitionTimer.Progress;
        }

        m_scrollRect.content.localPosition = Vector2.Lerp(m_startPosition, m_targetPosition, Easings.Ease(progress, m_easingFunction));
    }

    public void FocusOnChild(RectTransform child)
    {
        m_targetPosition = m_scrollRect.GetSnapToPositionToBringChildIntoView(child);
        m_startPosition = m_scrollRect.content.localPosition;

        m_transitionTimer.Reset();
        m_transitionTimer.Start();
    }
}
