using Sirenix.Utilities.Editor;

using UnityEditor;
using UnityEditor.Timeline;

using UnityEngine;
using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(SubTimelinePlayableAsset))]
public class SubTimelineClipEditor : ClipEditor
{
    public override ClipDrawOptions GetClipOptions(TimelineClip clip)
    {
        ClipDrawOptions options = base.GetClipOptions(clip);
        
        SubTimelinePlayableAsset asset = clip.asset as SubTimelinePlayableAsset;
        if (asset == null || asset.Timeline == null)
        {
            return options;
        }

        options.tooltip = asset.Timeline.name;

        return options;
    }

    public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
    {
        SubTimelinePlayableAsset asset = clip.asset as SubTimelinePlayableAsset;
        if (asset == null || asset.Timeline == null)
        {
            clip.displayName = "<NO ASSET OR TIMELINE ADDED>";
            return;
        }

        clip.displayName = $"{asset.Timeline.name}: {asset.m_shortDescription}";
        Rect rect = region.position;
        EditorGUI.DrawRect(rect, new Color(0, 0, 0));
    }
}
