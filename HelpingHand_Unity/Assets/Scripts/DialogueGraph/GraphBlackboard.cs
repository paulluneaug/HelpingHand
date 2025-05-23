using System.Collections.Generic;

using Sirenix.Serialization;

using UnityEngine;

using UnityUtility.SerializedDictionary;
using UnityUtility.Singletons;

public class GraphBlackboard : MonoBehaviourSingleton<GraphBlackboard>
{
    [OdinSerialize] [SerializeField]
    private Dictionary<string, object> m_blackboard = new();

    public Dictionary<string, object> Blackboard => m_blackboard;
    
}
