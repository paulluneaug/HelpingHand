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
    private IntVariable m_achievementsCount;

    [SerializeField]
    private BoolVariable m_isAchievementPlanelIsShowing;

    [SerializeField]
    private BoolVariable m_isAchievementDialogueIsPlaying;

    public bool IsAchievementIsPlaying => m_isAchievementPlanelIsShowing.Value;
    
    public bool IsAchievementDialogueIsPlaying => m_isAchievementDialogueIsPlaying.Value;

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
    private Dictionary<Achievement, Action> m_achivementActions;
  
    protected override void Start()
    {
        m_achivementActions = new Dictionary<Achievement, Action>();
        m_allAchievements = Resources.LoadAll<Achievement>("Achievements");
        foreach (Achievement achievement in m_allAchievements)
        {
            achievement.Init();
            Achievement a = achievement;
            m_achivementActions[achievement] = () => OnAchievementSet(a);
            achievement.OnEventRaised += m_achivementActions[achievement];
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
        foreach (Achievement achievement in m_allAchievements)
        {
            achievement.OnEventRaised -= m_achivementActions[achievement];
        }
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        base.OnDestroy();
    }

    private void OnAchievementSet(Achievement achievement)
    {
        SetAchievementAsync(achievement).Forget();
    }

    private async UniTaskVoid SetAchievementAsync(Achievement achievement)
    {
        m_isAchievementPlanelIsShowing.Value = true;
        SetAchievementTexts(achievement);
        m_achievementsCount.Value++;
        m_sfx.Post(gameObject);
        await ShowPanel();
        await UniTask.Delay((int)m_showDuration * 1000);
        await HidePanel();
        m_isAchievementPlanelIsShowing.Value = false;
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
}
