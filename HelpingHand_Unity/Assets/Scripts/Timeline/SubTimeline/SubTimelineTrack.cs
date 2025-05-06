using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(SubTimelinePlayableAsset))]
[TrackColor(0f, 0f, 0f)]
public class SubTimelineTrack : TrackAsset
{
    protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
    {
        Playable playable = base.CreatePlayable(graph, gameObject, clip);
        SubTimelinePlayableAsset asset = clip.asset as SubTimelinePlayableAsset;
        Debug.Log($"[SubTimelineTrack] <color=cyan>[{asset.Timeline.name}]</color> CreatePlayable");
        
        if (asset != null)
        {
            if (clip.GetParentTrack().parent is TimelineAsset timeline)
            {
                asset.m_parentTimeline = timeline;
            }
            else
            {
                Debug.LogError($"Clip is not part of a timeline asset");
                Debug.Break();
            }
            
            List<PreconditionTimeline> timelinePreconditions = asset.Precondition.SearchFor<PreconditionTimeline>();
            foreach (PreconditionTimeline timelinePrecondition in timelinePreconditions)
            {
                timelinePrecondition.m_start = clip.start;
                timelinePrecondition.m_end = clip.end;
            }
        }

        return playable;
    }
}
