using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Timeline;

using UnityUtility.Singletons;

public class TimelineManager : MonoBehaviourSingleton<TimelineManager>
{
    [SerializeField]
    private bool m_autoStart = false;
    
    [SerializeField]
    private TimelineRunner m_timelineRunnerPrefab;
    
    [SerializeField]
    private TimelineAsset m_sacredTimeline;

    public TimelineRunner CurrentRunner => m_currentRunner;

    private TimelineRunner m_sacredRunner;
    private TimelineRunner m_currentRunner;
    private Dictionary<TimelineAsset, bool> m_timelineReadStatuses = new();

    protected override void Start()
    {
        base.Start();
        if (m_autoStart)
        {
            m_sacredRunner = StartTimeline(m_sacredTimeline);
            m_currentRunner = m_sacredRunner;
        }
    }

    public TimelineRunner StartTimeline(TimelineAsset timeline)
    {
        Debug.Log($"[TimelineManager] <color=red>Starting runner [{timeline.name}]</color>");
        TimelineRunner newRunner = Instantiate(m_timelineRunnerPrefab);
        newRunner.name = $"Timeline Runner [{timeline.name}]";

        TimelineRunner previousRunner = m_currentRunner;
        if (previousRunner != null)
        {
            Debug.Log($"[TimelineManager] <color=red>Interrupting previous runner [{previousRunner.Timeline.name}]</color>");
            previousRunner.Pause();
            newRunner.OnTimelineCompleted += () =>
            {
                Debug.Log($"[TimelineManager] <color=red>Resuming runner [{previousRunner.Timeline.name}]</color>");
                m_currentRunner = previousRunner;
                previousRunner.Resume();
            };
        }
        
        m_currentRunner = newRunner;
        m_currentRunner.StartTimeline(timeline);
        m_timelineReadStatuses.Add(m_currentRunner.Timeline, true);
        
        return newRunner;
    }

    public bool HasBeenRead(TimelineAsset timeline)
    {
        if (m_timelineReadStatuses.TryGetValue(timeline, out bool hasBeenRead))
        {
            return hasBeenRead;
        }
        else
        {
            return false;
        }
    }
}