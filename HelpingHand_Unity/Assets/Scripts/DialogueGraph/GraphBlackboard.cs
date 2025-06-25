using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UnityEngine;

using UnityUtility.SerializedDictionary;
using UnityUtility.Singletons;

public class GraphBlackboard : MonoBehaviourSingleton<GraphBlackboard>
{
    [OdinSerialize]
    [SerializeField]
    [ReadOnly]
    private SerializedDictionary<string, object> m_blackboard = new();

    public SerializedDictionary<string, object> Blackboard => m_blackboard;

    public override void Initialize()
    {
        base.Initialize();
        m_blackboard = new();
    }
    
    // TODO reinitialize

    public bool TryGetValue<T>(string key, out T result)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            result = default;
            return false;
        }
#endif

        if (m_blackboard.TryGetValue(key, out object objectValue))
        {
            if (objectValue is T value)
            {
                result = value;
                return true;
            }
            else
            {
                throw new InvalidCastException($"Blackboard value corresponding to [{key}] is not of type {typeof(T).Name}");
            }
        }
        else
        {
            // throw new KeyNotFoundException($"Cannot find [{key}] in the blackboard");
            Debug.LogError($"Cannot find [{key}] in the blackboard");
        }

        result = default;
        return false;
    }
}
