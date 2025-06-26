using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievement")]
public class Achievement : EntityState
{
    public string title;
    public string body;

    [SerializeField]
    private ConditionBase m_condition = new ConditionNone();

    public void Init()
    {
        if (m_condition != null)
        {
            m_condition.Initialize();
            m_condition.OnPreconditionUpdated -= OnConditionUpdated;
            m_condition.OnPreconditionUpdated += OnConditionUpdated;
        }
    }

    private void OnConditionUpdated()
    {
        if (!IsActive)
        {
            return;
        }
        
        if (!IsSet && m_condition.Test())
        {
            Set();
            if (m_condition != null)
            {
                m_condition.OnPreconditionUpdated -= OnConditionUpdated;
            }
        }
    }
}
