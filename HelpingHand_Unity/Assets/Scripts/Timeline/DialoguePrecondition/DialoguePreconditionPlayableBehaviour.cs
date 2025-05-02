using UnityEngine;
using UnityEngine.Playables;

public class DialoguePreconditionPlayableBehaviour : PlayableBehaviour
{
    public Dialogue m_dialogue;
    public bool m_stayActive;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        m_dialogue.OnPreconditionUpdated();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (info.effectiveWeight > 0)
        {
            return;
        }
        // Don't trigger if the precondition stay true anyway
        if (!m_stayActive)
        {
            m_dialogue.OnPreconditionUpdated();
        }
    }
    
    // Called when the playable graph is created, typically when the timeline is played.
    public override void OnPlayableCreate(Playable playable)
    {
    }

    // Called when the playable is destroyed, typically when the timeline stops.
    public override void OnPlayableDestroy(Playable playable)
    {
    }
}