using System;
using System.Collections.Generic;
using System.Linq;

using Events;

using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using UnityUtility.Singletons;

using Object = UnityEngine.Object;

public class InputCountListenerSingleton : MonoBehaviourSingleton<InputCountListenerSingleton>
{
    [SerializeField]
    private List<BaseGameEvent> m_inputEvents;

    [SerializeField]
    private List<BaseGameEvent> m_physicalInputEvents;

    public List<BaseGameEvent> AllInputEvents => m_inputEvents.ToList();
    public List<BaseGameEvent> AllPhysicalInputEvents => m_physicalInputEvents.ToList();

    private readonly Dictionary<BaseGameEvent, Action> m_eventActions = new();
    private readonly Dictionary<BaseGameEvent, List<float>> m_eventTimes = new();
    private float m_nextWindowTime;

    public override void Initialize()
    {
    }

#if UNITY_EDITOR
    [Button("Load input events")]
    private void LoadAllInputEvents()
    {
        m_inputEvents = new List<BaseGameEvent>();
        m_physicalInputEvents = new List<BaseGameEvent>();
        var assetGUIDS = AssetDatabase.FindAssets("t:BaseGameEvent", new string[] { "Assets/Resources/InputEvents/" });
        foreach (string assetGUID in assetGUIDS)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            {
                if (obj is BaseGameEvent gameEvent)
                {
                    m_inputEvents.Add(gameEvent);
                    if (!AssetDatabase.IsSubAsset(gameEvent))
                    {
                        m_physicalInputEvents.Add(gameEvent);
                    }
                }
            }
        }
        AssetDatabase.SaveAssetIfDirty(this);
    }
#endif

    /// <summary>
    /// How many triggers from these inputs, for any input, in the time window provided?
    /// </summary>
    public int GetInputCount(IEnumerable<BaseGameEvent> inputEvents, float sinceTime, bool countAllTriggers = false)
    {
        int count = 0;
        foreach (BaseGameEvent inputEvent in inputEvents)
        {
            if (m_eventTimes.TryGetValue(inputEvent, out List<float> times))
            {
                foreach (float time in times)
                {
                    if (time > sinceTime)
                    {
                        count++;
                        if (!countAllTriggers)
                        {
                            break;
                        }
                    }
                }
            }
        }

        return count;
    }
    
    
    /// <summary>
    /// How many triggers from these inputs, for each inputs, in the time window provided?
    /// </summary>
    public List<(BaseGameEvent inputEvent, int count)> GetInputCounts(IEnumerable<BaseGameEvent> inputEvents, float sinceTime)
    {
        List<(BaseGameEvent inputEvent, int count)> result = new();
        foreach (BaseGameEvent inputEvent in inputEvents)
        {
            int count = 0;
            if (m_eventTimes.TryGetValue(inputEvent, out List<float> times))
            {
                foreach (float time in times)
                {
                    if (time > sinceTime)
                    {
                        count++;
                    }
                }
            }
            result.Add((inputEvent, count));
        }

        return result;
    }

    /// <summary>
    /// Closest of last input times from the list
    /// </summary>
    public float LastInputTime(IEnumerable<BaseGameEvent> inputEvents)
    {
        float maxTime = 0;
        foreach (BaseGameEvent inputEvent in inputEvents)
        {
            if (m_eventTimes.TryGetValue(inputEvent, out List<float> times))
            {
                if (times.Count > 0 && times[^1] > maxTime)
                {
                    maxTime = times[^1];
                }
            }
        }

        return maxTime;
    }

    private void OnEnable()
    {
        foreach (BaseGameEvent inputEvent in m_inputEvents)
        {
            BaseGameEvent evt = inputEvent;
            m_eventActions[evt] = () => OnInputUsed(evt); 
            evt.OnEventRaised -= m_eventActions[evt];
            evt.OnEventRaised += m_eventActions[evt];
        }
    }

    private void OnDisable()
    {
        foreach (BaseGameEvent inputEvent in m_inputEvents)
        {
            inputEvent.OnEventRaised -= m_eventActions[inputEvent];
        }
    }

    private void OnInputUsed(BaseGameEvent inputEvent)
    {
        if (!m_eventTimes.TryAdd(inputEvent, new List<float> { Time.time }))
        {
            m_eventTimes[inputEvent].Add(Time.time);
        }
    }
}
