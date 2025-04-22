using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Easings;
using UnityUtility.Extensions;
using UnityUtility.Timer;


public class InteractiveObjectTest : SingleSliderInteractiveObject
{
    public enum SlidingBehaviourEnum
    {
        Continuous,
        Discreet,
        SmoothDiscreet
    }

    public SlidingBehaviourEnum SlidingBehaviour { get => m_slidingBehaviour; set => m_slidingBehaviour = value; }
    public Vector3 StartPosition { get => m_startPosition; set => m_startPosition = value; }
    public Vector3 EndPosition { get => m_endPosition; set => m_endPosition = value; }
    public float SmoothDiscreetSlidingDuration 
    { 
        get => m_smoothDiscreetSlidingTimer.Duration; 
        set 
        {
            if (m_smoothDiscreetSlidingTimer.Duration != value)
            {
                m_smoothDiscreetSlidingTimer.Duration = value;
            }
        } 
    }
    public Easings.EasingFunction SmoothDircreetEasing
    {
        get => m_smoothDircreetEasing;
        set
        {
            if (m_smoothDircreetEasing != value)
            {
                m_smoothDircreetEasing = value;
            }
        }
    }

    [Range(0, 1)]
    [SerializeField] private float m_startValue;
    [SerializeField] private Vector3 m_startPosition;
    [SerializeField] private Vector3 m_endPosition;
    [SerializeField] private SlidingBehaviourEnum m_slidingBehaviour;

    [SerializeField, ShowIf(nameof(m_slidingBehaviour), SlidingBehaviourEnum.SmoothDiscreet)] private Timer m_smoothDiscreetSlidingTimer;
    [SerializeField, ShowIf(nameof(m_slidingBehaviour), SlidingBehaviourEnum.SmoothDiscreet)] private Easings.EasingFunction m_smoothDircreetEasing;

    [NonSerialized] private Vector3 m_currentSlidingTarget;
    [NonSerialized] private Vector3 m_currentSlidingStart;


    protected override void Start()
    {
        base.Start();
        float gridSize = PuppetSettings.Instance.TileSize;
        m_masterSlider.SliderValue = m_startValue;
        m_startPosition = m_startPosition.Snap(gridSize);
        m_endPosition = m_endPosition.Snap(gridSize);
    }

    private void Update()
    {
        float gridSize = PuppetSettings.Instance.TileSize;
        Vector3 targetPosition = Vector3.Lerp(m_startPosition, m_endPosition, m_masterSlider.SliderValue);

        switch (m_slidingBehaviour)
        {
            case SlidingBehaviourEnum.Continuous:
                transform.position = targetPosition;
                break;
            case SlidingBehaviourEnum.Discreet:
                transform.position = targetPosition.Snap(gridSize);
                break;
            case SlidingBehaviourEnum.SmoothDiscreet:
                targetPosition = targetPosition.Snap(gridSize);
                float progress = -1.0f;

                if (targetPosition == transform.position || m_smoothDiscreetSlidingTimer.Update(Time.deltaTime))
                {
                    m_smoothDiscreetSlidingTimer.Stop();
                    progress = 1.0f;
                }

                if (m_currentSlidingTarget != targetPosition)
                {
                    m_currentSlidingStart = transform.position;
                    m_currentSlidingTarget = targetPosition;

                    m_smoothDiscreetSlidingTimer.Reset();
                    m_smoothDiscreetSlidingTimer.Start();
                    progress = 0.0f;
                }

                progress = progress == -1.0f ? m_smoothDiscreetSlidingTimer.Progress : progress;

                transform.position = Vector3.Lerp(m_currentSlidingStart, m_currentSlidingTarget, Easings.Ease(progress, m_smoothDircreetEasing));

                break;
            default:
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(m_startPosition, m_endPosition);
    }
#endif
}
