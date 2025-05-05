using System;

using Sirenix.OdinInspector;

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
        {
            return;
        }

        if (m_precondition.Test() &&  (m_timelinePrecondition == null || m_timelinePrecondition.Test()))
        {
            HasBeenRead.Value = true;
            DialogueManager.Instance.PlayDialog(this);
        }
    }
}