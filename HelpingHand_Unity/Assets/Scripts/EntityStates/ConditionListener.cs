using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class ConditionListener : SerializedMonoBehaviour
{
    [SerializeField]
    [PropertySpace(0, 4)]
    [Required]
    private ConditionBase m_condition = new ConditionNone();

    [SerializeField]
    [FoldoutGroup("Callbacks")]
    private UnityEvent m_onConditionTrue;

    [SerializeField]
    [FoldoutGroup("Callbacks")]
    private UnityEvent m_onConditionFalse;

    private void OnEnable()
    {
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
    }

    private void OnDisable()
    {
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
    }

    private void OnConditionUpdated()
    {
        bool test = m_condition.Test();
        if (test)
        {
            m_onConditionTrue.Invoke();
        }
        else
        {
            m_onConditionFalse.Invoke();
        }
    }
}
