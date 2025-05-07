using System;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineRunner : MonoBehaviour
{
    public event Action OnTimelineStarted;
    public event Action OnTimelinePaused;
    public event Action OnTimelineResumed;
    public event Action OnTimelineCompleted;
    
    public TimelineAsset Timeline => m_timeline;
    public double Time => m_director.time;
    public double Duration => m_director.duration;

    private PlayableDirector m_director;
    private TimelineAsset m_timeline;
    private double m_interruptionTime;

    private void Awake()
    {
        m_director = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (m_director.state == PlayState.Paused)
        {
            return;
        }
        
        if (m_director.duration - m_director.time < double.Epsilon)
        {
            OnTimelineCompleted?.Invoke();
            Destroy(gameObject);
        }
    }

    public void StartTimeline(TimelineAsset timeline)
    {
        m_timeline = timeline;
        m_director.playableAsset = m_timeline;
        m_director.Play();
        OnTimelineStarted?.Invoke();
    }

    public void Pause()
    {
        m_director.Pause();
        OnTimelinePaused?.Invoke();
    }
    
    public void Resume()
    {
        m_director.Resume();
        OnTimelineResumed?.Invoke();
    }
}