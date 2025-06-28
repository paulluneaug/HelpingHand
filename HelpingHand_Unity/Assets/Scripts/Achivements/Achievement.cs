using System;

using Sirenix.OdinInspector;

using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievement")]
public class Achievement : EntityState
{
    public string title;
    public string body;

    [FoldoutGroup("Unlock conditions")]
    [SerializeField] 
    private int m_countCondition;
    
    [FoldoutGroup("Unlock conditions")]
    [NonSerialized]
    [ShowInInspector]
    [ReadOnly]
    private int m_currentCount;

    [FoldoutGroup("Unlock conditions")]
    [SerializeField]
    private ConditionBase m_condition = new ConditionNone();

    public void Init()
    {
        m_currentCount = 0;
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
            m_currentCount++;
            if (m_countCondition == m_currentCount)
            {
                AchievementManager.Instance.TrySetAchievement(this);
                if (m_condition != null)
                {
                    m_condition.OnPreconditionUpdated -= OnConditionUpdated;
                }
            }
        }
    }

    [Button("Try set achievement")]
    private void TrySetAchievement()
    {
        AchievementManager.Instance.TrySetAchievement(this);
    }
}
