using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class EntityStateSetter : SerializedMonoBehaviour
{
    [SerializeField][PropertySpace(0, 4)]
    private EntityState m_state;

    [SerializeField][BoxGroup][PropertySpace(4, 4)]
    private PreconditionBase m_precondition;

    [SerializeField][FoldoutGroup("Callbacks")]
    private UnityEvent m_onStateSet;

    [SerializeField][FoldoutGroup("Callbacks")]
    private UnityEvent m_onStateUnset;

    [SerializeField][FoldoutGroup("Callbacks")]
    private UnityEvent<bool> m_onStateChanged;

    private void Start()
    {
        m_precondition.Initialize();
        m_precondition.OnPreconditionUpdated += OnPreconditionUpdated;
        m_state.SetValueWithoutNotify(m_precondition.Test());
    }

    private void OnPreconditionUpdated()
    {
        bool test = m_precondition.Test();
        m_state.Value = test;
        if (test)
        {
            m_onStateSet.Invoke();
        }
        else
        {
            m_onStateUnset.Invoke();
        }
        m_onStateChanged.Invoke(test);
    }
}
