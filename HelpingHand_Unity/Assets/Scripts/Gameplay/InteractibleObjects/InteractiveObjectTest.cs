using System;

using UnityEngine;

using UnityUtility.Easings;


public class InteractiveObjectTest : MonoBehaviour
{
    [SerializeField] private SlidersManager.SliderIndex m_controllingSlider;

    [SerializeField] private Vector3 m_startPosition;
    [SerializeField] private Vector3 m_endPosition;
    [SerializeField] private Easings.EasingFunction m_easingFunction;

    [NonSerialized] private MasterSlider m_slider;


#if UNITY_EDITOR
    public Vector3 StartPosition { get => m_startPosition; set => m_startPosition = value; }
    public Vector3 EndPosition { get => m_endPosition; set => m_endPosition = value; }
#endif

    private void Start()
    {
        m_slider = GameManager.Instance.SlidersManager.GetSlider(m_controllingSlider);
    }


    private void Update()
    {
        transform.position = Vector3.Lerp(m_startPosition, m_endPosition, Easings.Ease(m_slider.Value, m_easingFunction));
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(m_startPosition, m_endPosition);
    }
#endif
}
