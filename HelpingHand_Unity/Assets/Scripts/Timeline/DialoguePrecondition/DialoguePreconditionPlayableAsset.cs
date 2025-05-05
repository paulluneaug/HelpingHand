using System;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialoguePreconditionPlayableAsset : PlayableAsset, ITimelineClipAsset
{
    [SerializeField]
    private bool m_stayActive;
    
    [InlineEditor(Expanded = true)]
    public Dialogue m_dialogue;

    public ClipCaps clipCaps => ClipCaps.None;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        ScriptPlayable<DialoguePreconditionPlayableBehaviour> playable = ScriptPlayable<DialoguePreconditionPlayableBehaviour>.Create(graph);
        
        DialoguePreconditionPlayableBehaviour behaviour = playable.GetBehaviour();
        behaviour.m_dialogue = m_dialogue;
        behaviour.m_stayActive = m_stayActive;
        
        if (m_dialogue != null)
        {
            if (m_dialogue.m_timelinePrecondition == null)
            {
                m_dialogue.m_timelinePrecondition = new PreconditionTimeline();
            }

            m_dialogue.m_timelinePrecondition.m_stayActive = m_stayActive;
        }
        return playable;
    }

    private void OnDestroy()
    {
        if (m_dialogue != null)
        {
            m_dialogue.m_timelinePrecondition = null;
        }
    }
}
