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
            m_condition.OnPreconditionUpdated += OnConditionUpdated;
        }
    }
    
    private void OnConditionUpdated()
    {
        if (!IsActive)
        {
            return;
        }
        
        if (!IsSet && !AchievementManager.Instance.IsAchievementIsPlaying && !AchievementManager.Instance.IsAchievementDialogueIsPlaying && m_condition.Test())
        {
            Set();
        
            if (m_condition != null)
            {
                m_condition.OnPreconditionUpdated -= OnConditionUpdated;
            }
        }
    }

    // Version: queuing achievements
    // private bool m_isWaitingToBeShown;
    // private void OnConditionUpdated()
    // {
    //     if (!IsActive)
    //     {
    //         return;
    //     }
    //     
    //     if (!IsSet && !m_isWaitingToBeShown && m_condition.Test())
    //     {
    //         SetAsync().Forget();
    //     }
    // }
    //
    // private async UniTaskVoid SetAsync()
    // {
    //     if (AchievementManager.Instance.IsAchievementCurrentlyShowing)
    //     {
    //         m_isWaitingToBeShown = true;
    //         await UniTask.WaitWhile(() => AchievementManager.Instance.IsAchievementCurrentlyShowing);
    //         await UniTask.Delay(1 * 1000);
    //     }
    //     
    //     Set();
    //     
    //     if (m_condition != null)
    //     {
    //         m_condition.OnPreconditionUpdated -= OnConditionUpdated;
    //     }
    //
    //     m_isWaitingToBeShown = false;
    // }
}
