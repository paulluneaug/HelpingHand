using System.Threading;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

[CreateAssetMenu]
public class AudioEvent : ScriptableObject
{
    [FoldoutGroup("Wwise Event")]
    [LabelWidth(75)]
    [SerializeField] private AK.Wwise.Event m_wwiseEvent;

    // === ONBOARDING ===
    [GUIColor(0.8f, 0.95f, 1f)]
    [FoldoutGroup("Onboarding")]
    [LabelText("Intro")]
    [SerializeField] private OnboardingIntroState m_onboardingIntroState;

    [GUIColor(0.8f, 0.95f, 1f)]
    [FoldoutGroup("Onboarding")]
    [LabelText("Curtain (Onboarding)")]
    [SerializeField] private OnboardingCurtainState m_onboardingCurtainState;

    [GUIColor(0.8f, 0.95f, 1f)]
    [FoldoutGroup("Onboarding")]
    [LabelText("Spot (Onboarding)")]
    [SerializeField] private OnboardingSpotState m_onboardingSpotState;

    // === INTERRUPTIONS ===
    [GUIColor(0.95f, 0.8f, 0.8f)]
    [FoldoutGroup("Interruptions")]
    [LabelText("Curtain (Interruption)")]
    [SerializeField] private InterruptionCurtainState m_interruptionCurtainState;

    [GUIColor(0.95f, 0.8f, 0.8f)]
    [FoldoutGroup("Interruptions")]
    [LabelText("Spot (Interruption)")]
    [SerializeField] private InterruptionSpotState m_interruptionSpotState;

    [GUIColor(0.95f, 0.8f, 0.8f)]
    [FoldoutGroup("Interruptions")]
    [LabelText("Roue (Interruption)")]
    [SerializeField] private InterruptionRoueState m_interruptionRoueState;

    [GUIColor(0.95f, 0.8f, 0.8f)]
    [FoldoutGroup("Interruptions")]
    [LabelText("Succes (Interruption)")]
    [SerializeField] private InterruptionSuccesState m_interruptionSuccesState;

    // === ACTE 1 ===
    [GUIColor(0.7f, 1f, 0.8f)]
    [FoldoutGroup("Acte 1")]
    [LabelText("Equipement")]
    [SerializeField] private EquipementState m_equipementState;

    [GUIColor(0.7f, 1f, 0.8f)]
    [FoldoutGroup("Acte 1")]
    [LabelText("Roue")]
    [SerializeField] private Acte1RoueState m_acte1roueState;

    [GUIColor(0.7f, 1f, 0.8f)]
    [FoldoutGroup("Acte 1")]
    [LabelText("Combat")]
    [SerializeField] private CombatState m_combatState;

    [GUIColor(0.7f, 1f, 0.8f)]
    [FoldoutGroup("Acte 1")]
    [LabelText("Fin")]
    [SerializeField] private FinState m_finState;

    //Succes
    [GUIColor(1f, 1f, 0.5f)]
    [FoldoutGroup("Succes")]
    [LabelText("Succes")]
    [SerializeField] private SuccesState m_succesState;


    [GUIColor(0.95f, 0.8f, 0.8f)]
    [FoldoutGroup("Interruptions")]
    [LabelText("Joystick (Interruption)")]
    [SerializeField] private InterruptionJoystickState m_interruptionjoystickState;

    [GUIColor(0.95f, 0.8f, 0.8f)]
    [FoldoutGroup("Interruptions")]
    [LabelText("Light Intensity (Interruption)")]
    [SerializeField] private InterruptionLightIntensityState m_interruptionlightintensityState;

    [GUIColor(0.7f, 1f, 0.8f)]
    [FoldoutGroup("Acte 1")]
    [LabelText("Combat2")]
    [SerializeField] private Combat2State m_combat2State;

    public async UniTask Play(GameObject target = null, CancellationToken cancellationToken = default)
    {
        if (m_wwiseEvent != null && m_wwiseEvent.IsValid())
        {
            await AudioManager.Instance.PostWwiseEventAsync(m_wwiseEvent, target, cancellationToken);
        }
        else
        {
            await AudioManager.Instance.PlayDialogueWithStatesAsync(
                m_onboardingIntroState.ToString(),
                m_onboardingCurtainState.ToString(),
                m_onboardingSpotState.ToString(),
                m_interruptionCurtainState.ToString(),
                m_interruptionSpotState.ToString(),
                m_acte1roueState.ToString(),
                m_combatState.ToString(),
                m_interruptionRoueState.ToString(),
                m_interruptionSuccesState.ToString(),

                m_equipementState.ToString(),
                m_finState.ToString(),
                m_succesState.ToString(),
                m_combat2State.ToString(),
                m_interruptionlightintensityState.ToString(),
                m_interruptionjoystickState.ToString(),


                target,
                cancellationToken
            );
        }
    }
}
