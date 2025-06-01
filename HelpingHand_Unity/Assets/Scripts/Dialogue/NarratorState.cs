#if UNITY_EDITOR
using UnityEditor;
#endif

using Sirenix.OdinInspector;

using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Narrator State")]
public class NarratorState : IntVariable
{
    [SerializeField]
    [LabelText("Satisfied")]
    [BoxGroup("Thresholds")]
    [HorizontalGroup("Thresholds/Horiz")]
    [VerticalGroup("Thresholds/Horiz/A")]
    private int m_satisfiedThreshold;
    
    [SerializeField]
    [LabelText("Neutral")]
    [VerticalGroup("Thresholds/Horiz/B")]
    [ReadOnly]
    private int m_neutralThreshold;
    
    [SerializeField]
    [LabelText("Annoyed")]
    [VerticalGroup("Thresholds/Horiz/C")]
    private int m_annoyedThreshold;
    
    [SerializeField]
    [LabelText("Pissed")]
    [VerticalGroup("Thresholds/Horiz/D")]
    private int m_pissedThreshold;

    [ShowInInspector]
    [ReadOnly]
    [Space]
    private string m_currentState = "Neutral";
    
    [SerializeField] 
    [HideInInspector]
    private EntityState m_stateNeutral;
    
    [SerializeField] 
    [HideInInspector]
    private EntityState m_stateSatisfied;
    
    [SerializeField] 
    [HideInInspector]
    private EntityState m_stateAnnoyed;
    
    [SerializeField] 
    [HideInInspector]
    private EntityState m_statePissed;

    public EntityState Neutral => m_stateNeutral;
    public EntityState Satisfied => m_stateSatisfied;
    public EntityState Annoyed => m_stateAnnoyed;
    public EntityState Pissed => m_statePissed;
    
#if UNITY_EDITOR
    private void Awake()
    {
        if (m_stateNeutral == null)
        {
            m_stateNeutral = ScriptableObject.CreateInstance<EntityState>();
            m_stateNeutral.name = $"{name}_StateNeutral";
            AssetDatabase.AddObjectToAsset(m_stateNeutral, this);
            m_stateSatisfied = ScriptableObject.CreateInstance<EntityState>();
            m_stateSatisfied.name = $"{name}_StateSatisfied";
            AssetDatabase.AddObjectToAsset(m_stateSatisfied, this);
            m_stateAnnoyed = ScriptableObject.CreateInstance<EntityState>();
            m_stateAnnoyed.name = $"{name}_StateAnnoyed";
            AssetDatabase.AddObjectToAsset(m_stateAnnoyed, this);
            m_statePissed = ScriptableObject.CreateInstance<EntityState>();
            m_statePissed.name = $"{name}_StatePissed";
            AssetDatabase.AddObjectToAsset(m_statePissed, this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }
#endif

    protected override void OnValueChanged(int oldValue, int newValue)
    {
        if (newValue <= m_satisfiedThreshold)
        {
            m_stateNeutral.Unset();
            m_stateSatisfied.Set();
            m_stateAnnoyed.Unset();
            m_statePissed.Unset();
            m_currentState = "Satisfied";
        }
        else if (newValue >= m_annoyedThreshold && newValue < m_pissedThreshold)
        {
            m_stateNeutral.Unset();
            m_stateSatisfied.Unset();
            m_stateAnnoyed.Set();
            m_statePissed.Unset();
            m_currentState = "Annoyed";
        }
        else if (newValue >= m_pissedThreshold)
        {
            m_stateNeutral.Unset();
            m_stateSatisfied.Unset();
            m_stateAnnoyed.Unset();
            m_statePissed.Set();
            m_currentState = "Pissed";
        }
        else
        {
            m_stateNeutral.Set();
            m_stateSatisfied.Unset();
            m_stateAnnoyed.Unset();
            m_statePissed.Unset();
            m_currentState = "Neutral";
        }
        base.OnValueChanged(oldValue, newValue);
    }
}
