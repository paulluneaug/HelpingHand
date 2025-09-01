using UnityEngine;
using UnityEngine.Events;

public class ConditionListenerCallbacks : ConditionListener
{
    [Space]
    [SerializeField]
    private UnityEvent m_onConditionTrue;

    [SerializeField]
    private UnityEvent m_onConditionFalse;

    protected override void OnConditionUpdated()
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
