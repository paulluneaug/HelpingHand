using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UnityUtility.Singletons;

[DefaultExecutionOrder(int.MaxValue)]
public class LateInitializationManager : MonoBehaviourSingleton<LateInitializationManager>
{
    private IEnumerable<ILateAwaker> m_lateAwakers;
    private IEnumerable<ILateStarter> m_lateStarters;
    
    public override void Initialize()
    {
        base.Initialize();
        m_lateAwakers = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ILateAwaker>();
        m_lateStarters = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ILateStarter>();
        
        foreach (ILateAwaker lateAwaker in m_lateAwakers)
        {
            lateAwaker.LateAwake();
        }
    }

    protected override void Start()
    {
        base.Start();
        foreach (ILateStarter lateStarter in m_lateStarters)
        {
            lateStarter.LateStart();
        }
    }
}
