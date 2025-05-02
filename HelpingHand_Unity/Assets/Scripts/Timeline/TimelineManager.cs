using UnityEngine;
using UnityEngine.Playables;

using UnityUtility.Singletons;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineManager : MonoBehaviourSingleton<TimelineManager>
{
    private PlayableDirector m_timeline;

    public override void Initialize()
    {
        base.Initialize();
        m_timeline = GetComponent<PlayableDirector>();
    }

    public void Play()
    {
        m_timeline.Play();
    }

    public void Pause()
    {
        m_timeline.Pause();
    }

    public double Time => m_timeline.time;
}
