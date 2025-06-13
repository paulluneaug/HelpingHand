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

    // === ONBOARDING ===
    [GUIColor(0.8f, 0.95f, 1f)]
    [BoxGroup("Audio/Onboarding", ShowLabel = true)]
    [LabelText("Intro")]
    [SerializeField] private OnboardingIntroState m_onboardingIntroState;


    [GUIColor(0.8f, 0.95f, 1f)]
    [BoxGroup("Audio/Onboarding")]
    [LabelText("Curtain (Onboarding)")]
    [SerializeField] private OnboardingCurtainState m_onboardingCurtainState;

    [GUIColor(0.8f, 0.95f, 1f)]
    [BoxGroup("Audio/Onboarding")]
    [LabelText("Spot (Onboarding)")]
    [SerializeField] private OnboardingSpotState m_onboardingSpotState;

    // === INTERRUPTIONS ===
    [GUIColor(0.95f, .8f, 0.8f)] // Couleur
    [BoxGroup("Audio/Interruptions", ShowLabel = true)]
    [LabelText("Curtain (Interruption)")]
    [SerializeField] private InterruptionCurtainState m_interruptionCurtainState;

    [GUIColor(0.95f, .8f, 0.8f)]
    [BoxGroup("Audio/Interruptions")]
    [LabelText("Spot (Interruption)")]
    [SerializeField] private InterruptionSpotState m_interruptionSpotState;

    // === ACTE 1 ===
    [GUIColor(0.7f, 1f, 0.8f)]
    [BoxGroup("Audio/Acte 1", ShowLabel = true)]
    [LabelText("Roue")]
    [SerializeField] private Acte1RoueState m_acte1roueState;

    // === ACTE x ===
    [GUIColor(0.7f, 1f, 0.8f)]
    [BoxGroup("Audio/Acte x")]
    [LabelText("Combat")]
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
