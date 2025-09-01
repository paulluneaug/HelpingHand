using Sirenix.OdinInspector;

using UnityEngine;

public abstract class ConditionListener : SerializedMonoBehaviour
{
    [SerializeField]
    [PropertySpace(0, 4)]
    [Required]
    protected ConditionBase m_condition = new ConditionNone();

    private void OnEnable()
    {
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
    }

    private void OnDisable()
    {
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
    }

    protected abstract void OnConditionUpdated();
}
