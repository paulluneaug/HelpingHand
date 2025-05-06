using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(DialogueTriggerPlayableAsset))]
[TrackColor(0.2641509f, 0, 0.1406891f)]
public class DialogueTriggerTrack : TrackAsset
{
    protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
    {
        Debug.Log($"DialogueTriggerTrack CreatePlayable");
        
        Playable playable = base.CreatePlayable(graph, gameObject, clip);

        DialogueTriggerPlayableAsset asset = clip.asset as DialogueTriggerPlayableAsset;
        if (asset != null)
        {
            asset.m_dialogue.m_clip = clip;
            if (clip.GetParentTrack().parent is TimelineAsset timeline)
            {
                asset.m_dialogue.m_parentTimeline = timeline;
            }
            else
            {
                Debug.LogError($"Clip is not part of a timeline asset");
                Debug.Break();
            }
            
            List<PreconditionTimeline> timelinePreconditions = asset.m_dialogue.Precondition.SearchFor<PreconditionTimeline>();
            foreach (PreconditionTimeline timelinePrecondition in timelinePreconditions)
            {
                timelinePrecondition.m_start = clip.start;
                timelinePrecondition.m_end = clip.end;
            }
        }

        return playable;
    }
}
