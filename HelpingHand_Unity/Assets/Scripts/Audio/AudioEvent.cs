using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

[CreateAssetMenu]
public class AudioEvent : ScriptableObject
{
    [SerializeField]
    [LabelWidth(75)]
    private AK.Wwise.Event m_wwiseEvent;

    [BoxGroup("Onboarding", ShowLabel = true)]
    [HorizontalGroup("Onboarding/States")]
    [LabelWidth(100)]
    [SerializeField] private OnboardingIntroState m_onboardingIntroState;

    [HorizontalGroup("Onboarding/States")]
    [LabelWidth(100)]
    [SerializeField] private OnboardingCurtainState m_onboardingCurtainState;

    [HorizontalGroup("Onboarding/States")]
    [LabelWidth(100)]
    [SerializeField] private OnboardingSpotState m_onboardingSpotState;

    [BoxGroup("Interruptions", ShowLabel = true)]
    [HorizontalGroup("Interruptions/States")]
    [HideLabel]
    [SerializeField] private InterruptionCurtainState m_interruptionCurtainState;

    [HorizontalGroup("Interruptions/States")]
    [HideLabel]
    [SerializeField] private InterruptionSpotState m_interruptionSpotState;

    [BoxGroup("Acte 1", ShowLabel = true)]
    [HorizontalGroup("Acte 1/States")]
    [LabelWidth(100)]
    [SerializeField] private Acte1RoueState m_acte1roueState;

    [HorizontalGroup("Acte 1/States")]
    [LabelWidth(100)]
    [SerializeField] private CombatState m_combatState;


    public async UniTask Play(GameObject target = null, CancellationToken cancellationToken = default)
    {
        if (m_wwiseEvent != null && m_wwiseEvent.IsValid())
        {
            await AudioManager.Instance.PostWwiseEventAsync(m_wwiseEvent, target, cancellationToken);
        }
        else
        {
            await AudioManager.Instance.PlayDialogueWithStatesAsync(m_onboardingIntroState.ToString(), m_onboardingCurtainState.ToString(), m_onboardingSpotState.ToString(), m_interruptionCurtainState.ToString(), m_interruptionSpotState.ToString(), m_acte1roueState.ToString(), m_combatState.ToString(), target, cancellationToken);
        }
    }
}
