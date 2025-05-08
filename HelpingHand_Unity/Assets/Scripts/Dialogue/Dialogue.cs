using System;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

//using Unity.VisualScripting.FullSerializer;

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
    [SerializeField] private NarraState narra;

    public enum RepetitionState { IGNORE, R1, R2, R3, R4 }
    public enum EtatState { IGNORE,On, Off }
    public enum ObjetState { IGNORE, Spot, Rideaux, Armure, Carton, Rien }
    public enum NarraState { IGNORE, Narra1, Narra2, Narra3, Narra4, Narra5, Narra6, Narra7, Narra8, Narra9, Narra10 }
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
                AudioManager.Instance.PlayDialogueWithStates(repetition.ToString(),etat.ToString(),objet.ToString(), narra.ToString());
                Debug.Log($"[Dialogue Triggered] {repetition}, {etat}, {objet}");
            }
        }
    }
}