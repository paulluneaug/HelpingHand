using UnityEditor.Timeline;

using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(DialoguePreconditionPlayableAsset))]
public class DialoguePreconditionClipEditor : ClipEditor
{
    public override ClipDrawOptions GetClipOptions(TimelineClip clip)
    {
        ClipDrawOptions options = base.GetClipOptions(clip);
        
        DialoguePreconditionPlayableAsset asset = clip.asset as DialoguePreconditionPlayableAsset;
        if (asset == null || asset.m_dialogue == null)
        {
            return options;
        }

        options.tooltip = asset.m_dialogue.name;

        return options;
    }

    public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
    {
        DialoguePreconditionPlayableAsset asset = clip.asset as DialoguePreconditionPlayableAsset;
        if (asset == null || asset.m_dialogue == null)
        {
            clip.displayName = "<NO ASSET OR DIALOGUE ADDED>";
            return;
        }

        clip.displayName = asset.m_dialogue.Content;
    }
}
