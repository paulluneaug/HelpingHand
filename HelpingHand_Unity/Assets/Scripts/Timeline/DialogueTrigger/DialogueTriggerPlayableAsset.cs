using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueTriggerPlayableAsset : PlayableAsset, ITimelineClipAsset
{
    [InlineEditor(Expanded = true)]
    public Dialogue m_dialogue;
    
    public ClipCaps clipCaps => ClipCaps.None;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        ScriptPlayable<DialogueTriggerPlayableBehaviour> playable = ScriptPlayable<DialogueTriggerPlayableBehaviour>.Create(graph);

        DialogueTriggerPlayableBehaviour behaviour = playable.GetBehaviour();
        behaviour.m_dialogue = m_dialogue;

        return playable;
    }
}
