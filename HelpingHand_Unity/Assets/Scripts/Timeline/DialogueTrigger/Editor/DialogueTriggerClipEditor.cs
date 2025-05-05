using UnityEditor;
using UnityEditor.Timeline;

using UnityEngine;
using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(DialogueTriggerPlayableAsset))]
public class DialogueTriggerClipEditor : ClipEditor
{
    public override ClipDrawOptions GetClipOptions(TimelineClip clip)
    {
        ClipDrawOptions options = base.GetClipOptions(clip);
        
        DialogueTriggerPlayableAsset asset = clip.asset as DialogueTriggerPlayableAsset;
        if (asset == null || asset.m_dialogue == null)
        {
            return options;
        }

        options.tooltip = asset.m_dialogue.name;

        return options;
    }

    public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
    {
        DialogueTriggerPlayableAsset asset = clip.asset as DialogueTriggerPlayableAsset;
        if (asset == null || asset.m_dialogue == null)
        {
            clip.displayName = "<NO ASSET OR DIALOGUE ADDED>";
            return;
        }

        clip.displayName = asset.m_dialogue.Content;
        Rect rect = region.position;
        EditorGUI.DrawRect(rect, new Color(0.2641509f, 0, 0.1406891f));
    }
}
