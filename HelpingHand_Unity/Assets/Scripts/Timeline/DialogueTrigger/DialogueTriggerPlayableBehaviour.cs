using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueTriggerPlayableBehaviour : PlayableBehaviour
{
    public Dialogue m_dialogue;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (!Application.isPlaying)
        {
            return;
        }
        
        Debug.Log($"[OnBehaviourPlay] <color=green>[{m_dialogue.name}]</color> triggered");
        m_dialogue.TriggerPreconditionsTests();
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