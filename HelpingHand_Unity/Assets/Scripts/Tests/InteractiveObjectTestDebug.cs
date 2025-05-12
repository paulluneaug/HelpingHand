using UnityEngine;

using UnityUtility.Easings;

public class InteractiveObjectTestDebug : MonoBehaviour
{
    [SerializeField] private SlidingBehaviourDebugFieldController m_slidingBehaviourController;
    [SerializeField] private EasingFunctionDebugFieldController m_easingFunctionController;
    [SerializeField] private SliderDebugFieldController m_slidingDurationController;

    [SerializeField] private InteractiveObjectTest m_target;


    private void Awake()
    {
        m_slidingBehaviourController.Init("Sliding behaviour", m_target.SlidingBehaviour);
        m_easingFunctionController.Init("Easing", m_target.SmoothDircreetEasing);
        m_slidingDurationController.Init("Sliding Duration", m_target.SmoothDiscreetSlidingDuration);
    }

    private void Update()
    {
        m_target.SlidingBehaviour = m_slidingBehaviourController.GetValue();
        m_target.SmoothDircreetEasing = m_easingFunctionController.GetValue();
        m_target.SmoothDiscreetSlidingDuration = m_slidingDurationController.GetValue();
    }
}
