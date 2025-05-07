using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[ShowOdinSerializedPropertiesInInspector]
public class SubTimelinePlayableAsset : PlayableAsset, ITimelineClipAsset, ISerializationCallbackReceiver, ISupportsPrefabSerialization
{
    [SerializeField]
    private TimelineAsset m_timeline;

    [PropertySpace(8, 4)] [SerializeField]
    public string m_shortDescription;
    
    [PropertySpace(4, 4)] [SerializeField] [BoxGroup]
    private PreconditionBase m_precondition = new PreconditionNone();

    [Space] [SerializeField]
    private bool m_interruptOnce = true;
    public TimelineAsset m_parentTimeline;

    public TimelineAsset Timeline => m_timeline;
    public PreconditionBase Precondition => m_precondition;
    public ClipCaps clipCaps => ClipCaps.None;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        Debug.Log($"[SubTimelinePlayableAsset] <color=cyan>[{m_timeline.name}]</color> CreatePlayable");
        ScriptPlayable<SubTimelinePlayableBehaviour> playable = ScriptPlayable<SubTimelinePlayableBehaviour>.Create(graph);
        SubTimelinePlayableBehaviour behaviour = playable.GetBehaviour();
        behaviour.m_timeline = m_timeline;
        behaviour.m_precondition = m_precondition;
        behaviour.m_parentTimeline = m_parentTimeline;
        behaviour.m_interruptOnce = m_interruptOnce;
        behaviour.m_asset = this;

        return playable;
    }

    #region Odin Serialization

    [SerializeField, HideInInspector]
    private SerializationData m_serializationData;

    public void OnBeforeSerialize()
    {
        UnitySerializationUtility.SerializeUnityObject(this, ref m_serializationData);
    }

    public void OnAfterDeserialize()
    {
        UnitySerializationUtility.DeserializeUnityObject(this, ref m_serializationData);
    }

    public SerializationData SerializationData { get => m_serializationData; set => m_serializationData = value; }

    #endregion
}
