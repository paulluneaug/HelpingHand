using System;

using Events;

using Sirenix.OdinInspector;

using UnityEngine;

[System.Serializable]
public class ConditionLastInputSince : ConditionBase
{
    private enum InputListType
    {
        [LabelText("Any input")] AnyInput,
        [LabelText("Only these inputs")] OnlyThese,
    }
    
    [SerializeField] [LabelWidth(100)]
    private float m_timeWindow;
    
    [SerializeField] [EnumToggleButtons] [LabelWidth(100)] [OnValueChanged("OnTypeChanged")]
    private InputListType m_type;

    private void OnTypeChanged()
    {
        m_inputEvents = Array.Empty<BaseGameEvent>();
    }

    [SerializeField] [ShowIf("@m_type == InputListType.OnlyThese")] [PropertySpace(8, 8)]
    private BaseGameEvent[] m_inputEvents = Array.Empty<BaseGameEvent>();

    public override void Initialize()
    {
        base.Initialize();
        StandaloneTimerSingleton.Instance.OnUpdateTickEvent -= RaiseOnPreconditionUpdated;
        StandaloneTimerSingleton.Instance.OnUpdateTickEvent += RaiseOnPreconditionUpdated;
    }

    public override void Dispose()
    {
        StandaloneTimerSingleton.Instance.OnUpdateTickEvent -= RaiseOnPreconditionUpdated;
    }

    public override bool Test()
    {
        return Time.time - (m_type switch
        {
            InputListType.AnyInput => InputCountListenerSingleton.Instance.LastInputTime(),
            InputListType.OnlyThese => InputCountListenerSingleton.Instance.LastInputTime(m_inputEvents),
            _ => throw new ArgumentOutOfRangeException()
        }) > m_timeWindow;
    }
}
