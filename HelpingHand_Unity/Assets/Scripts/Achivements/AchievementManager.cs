using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;

using UnityUtility.Singletons;

public class AchievementManager : MonoBehaviourSingleton<AchievementManager>
{
    [SerializeField]
    private bool m_startHidden = true;

    [SerializeField]
    private AK.Wwise.Event m_sfx;

    [SerializeField] 
    private float m_delayBetweenAchievements = .5f;

    [SerializeField]
    private IntVariable m_achievementsCount;

    [FoldoutGroup("Texts")]
    [SerializeField]
    private TMP_Text m_titleText;

    [FoldoutGroup("Texts")]
    [SerializeField]
    private TMP_Text m_bodyText;

    [FoldoutGroup("Animations")]
    [SerializeField]
    private float m_showDuration = 2f;

    [FoldoutGroup("Animations")]
    [SerializeField]
    private Transform m_transform;

    [BoxGroup("Animations/Show")]
    [SerializeField]
    private float m_showY = 2.6f;

    [BoxGroup("Animations/Show")]
    [SerializeField]
    private float m_showAnimationDuration = .5f;

    [BoxGroup("Animations/Show")]
    [SerializeField]
    private Ease m_showAnimationEase = Ease.OutBounce;

    [BoxGroup("Animations/Hide")]
    [SerializeField]
    private float m_hideY = 5f;

    [BoxGroup("Animations/Hide")]
    [SerializeField]
    private float m_hideAnimationDuration = .5f;

    [BoxGroup("Animations/Hide")]
    [SerializeField]
    private Ease m_hideAnimationEase = Ease.InQuad;

    private Achievement[] m_allAchievements;
    private Queue<Achievement> m_achievementsQueue;
    private Achievement m_currentShowingAchievement;

    protected override void Start()
    {
        m_achievementsQueue = new Queue<Achievement>();
        m_allAchievements = Resources.LoadAll<Achievement>("Achievements");
        foreach (Achievement achievement in m_allAchievements)
        {
            achievement.Init();
        }

        m_transform.localPosition = new Vector3(m_transform.localPosition.x, m_startHidden ? m_hideY : m_showY, m_transform.localPosition.z);

        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        OnGameStateChanged(GameManager.Instance.CurrentGameState);
    }

    private void OnGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.MainMenu:
                SetEnableAllAchievements(false);
                break;
            case GameManager.GameState.Gameplay:
                SetEnableAllAchievements(true);
                break;
        }
    }

    private void SetEnableAllAchievements(bool isEnabled)
    {
        foreach (Achievement achievement in m_allAchievements)
        {
            achievement.IsActive = isEnabled;
        }
    }

    public override void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        base.OnDestroy();
    }

    private void OnAchievementSet(Achievement achievement)
    {
    }



    private void SetAchievementTexts(Achievement achievement)
    {
        m_titleText.text = achievement.title;
        m_bodyText.text = achievement.body;
    }

    private async UniTask ShowPanel()
    {
        await m_transform.DOLocalMoveY(m_showY, m_showAnimationDuration).SetEase(m_showAnimationEase).ToUniTask();
    }

    private async UniTask HidePanel()
    {
        await m_transform.DOLocalMoveY(m_hideY, m_hideAnimationDuration).SetEase(m_hideAnimationEase).ToUniTask();
    }

    public void TrySetAchievement(Achievement achievement)
    {
        if (m_currentShowingAchievement != null)
        {
            m_achievementsQueue.Enqueue(achievement);
        }
        else
        {
            SetAchievement(achievement);
        }
    }

    private void SetAchievement(Achievement achievement)
    {
        SetAchievementAsync(achievement).Forget();
    }
    
    private async UniTaskVoid SetAchievementAsync(Achievement achievement)
    {
        SetAchievementTexts(achievement);
        m_achievementsCount.Value++;
        m_sfx.Post(gameObject);
        await ShowPanel();
        achievement.Set();
        await UniTask.Delay((int)m_showDuration * 1000);
        await HidePanel();
        if (m_achievementsQueue.Count > 0)
        {
            await UniTask.Delay((int)m_delayBetweenAchievements * 1000);
            SetAchievementAsync(m_achievementsQueue.Dequeue()).Forget();
        }
    }
}
