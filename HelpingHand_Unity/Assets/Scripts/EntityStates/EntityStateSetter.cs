using Sirenix.OdinInspector;

using UnityEngine;

public class EntityStateSetter : SerializedMonoBehaviour
{
    [SerializeField]
    [PropertySpace(0, 4)]
    private EntityState m_state;

    [SerializeField]
    [BoxGroup]
    [PropertySpace(4, 4)]
    private ConditionBase m_condition = new ConditionNone();

    private void Start()
    {
        if (m_state == null)
        {
            Debug.LogWarning($"State is null", gameObject);
            return;
        }
        m_condition.Initialize();
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
        m_state.SetValueWithoutNotify(m_condition.Test());
    }

    private void OnConditionUpdated()
    {
        if (m_state == null)
        {
            Debug.LogWarning($"State is null", gameObject);
            return;
        }
        m_state.Value = m_condition.Test();
    }
}
