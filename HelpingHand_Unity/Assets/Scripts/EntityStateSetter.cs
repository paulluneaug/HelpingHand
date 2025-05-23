using Sirenix.OdinInspector;

using UnityEngine;

public class EntityStateSetter : SerializedMonoBehaviour
{
    [SerializeField][PropertySpace(0, 4)]
    private EntityState m_state;

    [SerializeField][BoxGroup][PropertySpace(4, 4)]
    private ConditionBase m_condition = new ConditionNone();

    private void Start()
    {
        m_condition.Initialize();
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
        m_state.SetValueWithoutNotify(m_condition.Test());
    }

    private void OnConditionUpdated()
    {
        m_state.Value  = m_condition.Test();
    }
}
