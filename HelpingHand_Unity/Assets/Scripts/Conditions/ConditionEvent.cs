using Cysharp.Threading.Tasks;

using Events;

using UnityEngine;

[System.Serializable]
public class ConditionEvent : ConditionBase
{
    [SerializeField]
    private BaseGameEvent m_event;

    private bool m_isEventRaised;
    
    public override void Initialize()
    {
        base.Initialize();
        m_isEventRaised = false;
        m_event.RemoveListener(OnEventRaised);
        m_event.AddListener(OnEventRaised);
    }

    public override void Dispose()
    {
        base.Dispose();
        m_event.RemoveListener(OnEventRaised);
    }

    public override bool Test()
    {
        return m_isEventRaised;
    }

    private void OnEventRaised()
    {
        m_isEventRaised = true;
        RaiseOnPreconditionUpdated();
        // The event is raised only for 1 frame
        UniTask.Action(async () =>
        {
            await UniTask.NextFrame();
            m_isEventRaised = false;
        }).Invoke();
    }
}