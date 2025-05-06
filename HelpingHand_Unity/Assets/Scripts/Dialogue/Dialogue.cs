using System;

using Sirenix.OdinInspector;

using Unity.VisualScripting.FullSerializer;

using UnityEngine;

using UnityUtility.ObservableFields;

[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue/Dialogue")]
public class Dialogue : SerializedScriptableObject
{
    [SerializeField]
    private PreconditionBase m_precondition = new PreconditionNone();

    [NonSerialized]
    public PreconditionTimeline m_timelinePrecondition;

    [SerializeField]
    private int m_priority;

    [SerializeField]
    private bool m_canBeReadMultipleTimes = false;

    [SerializeField, TextArea(5, 5)]
    private string m_content;

    public string Content => m_content;
    public int Priority => m_priority;
    public ObservableField<bool> HasBeenRead { get; } = new ObservableField<bool>(false);

    [SerializeField] private RepetitionState repetition;
    [SerializeField] private EtatState etat;
    [SerializeField] private ObjetState objet;

    public enum RepetitionState { R1, R2, R3, R4 }
    public enum EtatState { On, Off }
    public enum ObjetState { Spot, Rideaux, Armure, Carton, Rien }

    public void Initialize()
    {
        HasBeenRead.Value = false;
        m_precondition.Initialize();
        m_precondition.OnPreconditionUpdated -= OnPreconditionUpdated;
        m_precondition.OnPreconditionUpdated += OnPreconditionUpdated;
        if (m_timelinePrecondition != null)
        {
            m_timelinePrecondition.OnPreconditionUpdated -= OnPreconditionUpdated;
            m_timelinePrecondition.OnPreconditionUpdated += OnPreconditionUpdated;
        }
    }

    public void OnPreconditionUpdated()
    {
        if (HasBeenRead.Value && !m_canBeReadMultipleTimes)
            return;

        if (m_precondition.Test() && (m_timelinePrecondition == null || m_timelinePrecondition.Test()))
        {
            HasBeenRead.Value = true;

            // Appel AudioManager avec les bons noms de state (match Wwise !)
            AudioManager.Instance.PlayDialogueWithStates(
                repetition.ToString(),
                etat.ToString(),
                objet.ToString()
            );

            Debug.Log($"[Dialogue Triggered] {repetition}, {etat}, {objet}");
        }
    }
}