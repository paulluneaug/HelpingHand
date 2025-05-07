using System;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using Unity.VisualScripting.FullSerializer;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

using UnityUtility.ObservableFields;

[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue/Dialogue")]
public class Dialogue : SerializedScriptableObject
{
    [SerializeField][BoxGroup]
    private PreconditionBase m_precondition = new PreconditionNone();

    [Space][SerializeField]
    private int m_priority;

    [SerializeField]
    private bool m_canBeReadMultipleTimes = false;

    [SerializeField]
    private bool m_canBeInterrupted = false;

    [Space][SerializeField, TextArea(3, 3)]
    private string m_content;
    
    [Space]
    public TimelineAsset m_parentTimeline;

    public PreconditionBase Precondition => m_precondition;
    public int Priority => m_priority;
    public string Content => m_content;
    public bool CanBeInterrupted => m_canBeInterrupted;
    public ObservableField<bool> HasBeenRead { get; } = new ObservableField<bool>(false);

    [SerializeField] private RepetitionState repetition;
    [SerializeField] private EtatState etat;
    [SerializeField] private ObjetState objet;

    public enum RepetitionState { R1, R2, R3, R4 }
    public enum EtatState { On, Off }
    public enum ObjetState { Spot, Rideaux, Armure, Carton, Rien }
    [HideInInspector]
    public TimelineClip m_clip;

    public void Initialize()
    {
        HasBeenRead.Value = false;
        m_precondition.Initialize();
        m_precondition.OnPreconditionUpdated -= TriggerPreconditionsTests;
        m_precondition.OnPreconditionUpdated += TriggerPreconditionsTests;
    }

    public void TriggerPreconditionsTests()
    {
        // Debug.Log($"[TriggerPreconditionsTests] <color=green>[{name}]</color> hasBeenRead={HasBeenRead.Value} canBeReadMultipleTimes={m_canBeReadMultipleTimes} parentTimeline={m_parentTimeline.name} currentTimeline={TimelineManager.Instance.CurrentRunner.Timeline}");
        if (HasBeenRead.Value && !m_canBeReadMultipleTimes)
            return;

        if (m_parentTimeline == null || (TimelineManager.Instance.CurrentRunner != null && m_parentTimeline == TimelineManager.Instance.CurrentRunner.Timeline))
        {
            if (m_precondition.Test())
            {
                DialogueManager.Instance.PlayDialog(this);
                AudioManager.Instance.PlayDialogueWithStates(repetition.ToString(),etat.ToString(),objet.ToString());
                Debug.Log($"[Dialogue Triggered] {repetition}, {etat}, {objet}");
            }
        }
    }
}