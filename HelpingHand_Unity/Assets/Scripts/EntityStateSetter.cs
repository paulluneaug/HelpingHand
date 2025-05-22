using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class EntityStateSetter : SerializedMonoBehaviour
{
    [SerializeField][PropertySpace(0, 4)]
    private EntityState m_state;

    [SerializeField][BoxGroup][PropertySpace(4, 4)]
    private ConditionBase m_condition = new ConditionNone();

    [SerializeField][FoldoutGroup("Callbacks")]
    private UnityEvent m_onStateSet;

    [SerializeField][FoldoutGroup("Callbacks")]
    private UnityEvent m_onStateUnset;

    [SerializeField][FoldoutGroup("Callbacks")]
    private UnityEvent<bool> m_onStateChanged;

    private void Start()
    {
        m_condition.Initialize();
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
        m_state.SetValueWithoutNotify(m_condition.Test());
        m_state.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool value)
    {
        if (value)
        {
            m_onStateSet.Invoke();
        }
        else
        {
            m_onStateUnset.Invoke();
        }
        m_onStateChanged.Invoke(value);
    }

    private void OnConditionUpdated()
    {
        m_state.Value  = m_condition.Test();
    }
}
