using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.ObservableFields;

[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue/Dialogue")]
public class Dialogue : SerializedScriptableObject
{
    [SerializeField]
    private PreconditionBase m_precondition;

    [SerializeField]
    private int m_priority;

    [SerializeField]
    private bool m_canBeReadMultipleTimes = false;

    [SerializeField, TextArea(5, 5)]
    private string m_content;

    public string Content => m_content;
    public int Priority => m_priority;
    public ObservableField<bool> HasBeenRead => m_hasBeenRead;

    private ObservableField<bool> m_hasBeenRead;

    public void Initialize()
    {
        m_hasBeenRead.Value = false;
        m_precondition.Initialize();
        m_precondition.OnPreconditionUpdated += OnPreconditionUpdated;
    }

    private void OnPreconditionUpdated()
    {
        if (m_hasBeenRead.Value && !m_canBeReadMultipleTimes)
        {
            return;
        }
        if (m_precondition.Test())
        {
            m_hasBeenRead.Value = true;
            DialogueManager.Instance.PlayDialog(this);
        }
    }
}