using Cysharp.Threading.Tasks;

using Events;

using Sirenix.OdinInspector;

using UnityEngine;

public class SteveController : SerializedMonoBehaviour
{
    [SerializeField]
    private ConditionBase m_condition = new ConditionNone();

    [SerializeField]
    private GameEvent m_steveEvent;

    [SerializeField]
    private EntityState m_steveAchievement;

    [SerializeField]
    private float m_showSteveDuration = 2;

    [SerializeField]
    private EntityState[] m_steveStates;

    [SerializeField]
    private EntityState[] m_steveTrapDoorStates;

    private int m_currentIndex = -1;
    private bool m_isShowingSteve;

    private void OnEnable()
    {
        m_condition.Initialize();
        m_condition.OnPreconditionUpdated += OnConditionUpdated;
    }

    private void OnDisable()
    {
        m_condition.OnPreconditionUpdated -= OnConditionUpdated;
    }

    private void OnConditionUpdated()
    {
        if (!m_isShowingSteve && m_condition.Test())
        {
            if (!AchievementManager.Instance.IsAchievementDialogueIsPlaying && !AchievementManager.Instance.IsAchievementIsPlaying && !m_steveAchievement.IsSet)
            {
                m_steveAchievement.Set();
            }

            ShowSteveAsync().Forget();
        }
    }

    private async UniTaskVoid ShowSteveAsync()
    {
        m_isShowingSteve = true;
        m_currentIndex = (m_currentIndex + 1) % m_steveStates.Length;
        EntityState steve = m_steveStates[m_currentIndex];
        EntityState trapDoor = m_steveTrapDoorStates[m_currentIndex];
        m_steveEvent.Raise();
        trapDoor.Set();
        await UniTask.Delay(300);
        steve.Set();
        await UniTask.Delay((int)(m_showSteveDuration * 1000));
        steve.Unset();
        await UniTask.Delay(300);
        trapDoor.Unset();
        await UniTask.Delay(500);
        m_isShowingSteve = false;
    }
}