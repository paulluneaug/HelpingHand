using UnityEngine;
using UnityEngine.Playables;

public class DialogueTriggerPlayableBehaviour : PlayableBehaviour
{
    public Dialogue m_dialogue;

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
    }
}