using UnityEngine;

using UnityUtility.Easings;


public class InteractiveObjectTest : MonoBehaviour
{
    [SerializeField] private MasterSlider m_controllingSlider;

    [SerializeField] private Vector3 m_startPosition;
    [SerializeField] private Vector3 m_endPosition;
    [SerializeField] private Easings.EasingFunction m_easingFunction;


#if UNITY_EDITOR
    public Vector3 StartPosition { get => m_startPosition; set => m_startPosition = value; }
    public Vector3 EndPosition { get => m_endPosition; set => m_endPosition = value; }
#endif


    private void Update()
    {
        transform.position = Vector3.Lerp(m_startPosition, m_endPosition, Easings.Ease(m_controllingSlider.Value, m_easingFunction));
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(m_startPosition, m_endPosition);
    }
#endif
}
