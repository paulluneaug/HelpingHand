using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(DialoguePreconditionPlayableAsset))] [TrackColor(1f, 1f, 1f)]
public class DialoguePreconditionTrack : TrackAsset
{
    protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
    {
        Playable playable = base.CreatePlayable(graph, gameObject, clip);

        DialoguePreconditionPlayableAsset asset = clip.asset as DialoguePreconditionPlayableAsset;
        if (asset != null && asset.m_dialogue != null)
        {
            asset.m_dialogue.m_timelinePrecondition.m_start = clip.start;
            asset.m_dialogue.m_timelinePrecondition.m_end = clip.end;
        }

        return playable;
    }
}