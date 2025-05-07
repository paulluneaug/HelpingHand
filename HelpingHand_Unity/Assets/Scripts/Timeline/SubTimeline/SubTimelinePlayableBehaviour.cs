using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SubTimelinePlayableBehaviour : PlayableBehaviour
{
    public TimelineAsset m_timeline;
    public PreconditionBase m_precondition;
    public TimelineAsset m_parentTimeline;
    public bool m_interruptOnce;
    public SubTimelinePlayableAsset m_asset;

    public override void OnGraphStart(Playable playable)
    {
        if (!Application.isPlaying)
        {
            return;
        }
        
        m_precondition.Initialize();
        m_precondition.OnPreconditionUpdated -= OnPreconditionUpdated;
        m_precondition.OnPreconditionUpdated += OnPreconditionUpdated;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Debug.Log($"[SubTimelinePlayableBehaviour] <color=cyan>[{m_timeline.name}]</color> OnBehaviourPlay");
        OnPreconditionUpdated();
    }

    private void OnPreconditionUpdated()
    {
        Debug.Log($"[SubTimelinePlayableBehaviour] <color=cyan>[{m_timeline.name}]</color> OnPreconditionUpdated parentTimeline={m_parentTimeline.name} CurrentTimeline={TimelineManager.Instance.CurrentRunner.Timeline.name} m_interruptOnce={m_interruptOnce} hasBeenRead={TimelineManager.Instance.HasBeenRead(m_timeline)} preconditions={ m_precondition.Test()}");
        if (m_parentTimeline == TimelineManager.Instance.CurrentRunner.Timeline)
        {
            if ((!m_interruptOnce || !TimelineManager.Instance.HasBeenRead(m_timeline)) && m_precondition.Test())
            {
                if (DialogueManager.Instance.CanBeInterrupted)
                {
                    if (DialogueManager.Instance.IsShowingText)
                    {
                        DialogueManager.Instance.Interrupt();
                    }
                    TimelineManager.Instance.StartTimeline(m_timeline);
                }
            }
        }
        else
        {
            Debug.Log($"{m_timeline.name}: wrong timeline");
        }
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