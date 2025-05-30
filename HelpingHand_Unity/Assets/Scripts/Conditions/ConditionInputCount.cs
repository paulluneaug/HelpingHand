using System;
using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using Events;

using Sirenix.OdinInspector;

using UnityEngine;

[Serializable]
public class ConditionInputCount : ConditionBase
{
    private enum InputListType
    {
        [LabelText("Any input")] AnyInput,
        [LabelText("Only these inputs")] OnlyThese,
        [LabelText("Any but these inputs")] AnyButThese,
    }

    private enum InputType
    {
        [LabelText("Triggers count")] Triggers,
        [LabelText("Inputs count")] Inputs,
    }

    private enum TimeType
    {
        [LabelText("Time window")] TimeWindow,
        [LabelText("Start time")] TimeStart
    }

    private enum StartTimeType
    {
        [LabelText("Global variable")] GlobalVariable,
        [LabelText("Blackboard")] Blackboard
    }
    
    [SerializeField] [LabelWidth(100)]
    private int m_count;

    [SerializeField] [LabelWidth(100)] [EnumToggleButtons]
    private TimeType m_timeType;
    
    [SerializeField] [LabelWidth(100)] [ShowIf("@m_timeType == TimeType.TimeWindow")]
    private float m_timeWindow;

    [SerializeField] [LabelWidth(100)] [EnumToggleButtons] [ShowIf("@m_timeType == TimeType.TimeStart")]
    private StartTimeType m_startTimeType;
    
    [SerializeField] [LabelWidth(100)] [ShowIf("@m_timeType == TimeType.TimeStart && m_startTimeType == StartTimeType.GlobalVariable")]
    private FloatVariable m_floatVariable;
    
    [SerializeField] [LabelWidth(100)] [ShowIf("@m_timeType == TimeType.TimeStart && m_startTimeType == StartTimeType.Blackboard")]
    private string m_blackboardKey;

    [SerializeField] [EnumToggleButtons] [LabelWidth(100)] [OnValueChanged("OnTypeChanged")]
    private InputListType m_type;

    private void OnTypeChanged()
    {
        m_onlyEvents = Array.Empty<BaseGameEvent>();
        m_anyButEvents = Array.Empty<BaseGameEvent>();
    }

    [SerializeField]
    [ShowIf("@m_type == InputListType.AnyInput")]
    [LabelWidth(100)]
    private bool m_usePhysicalInputs = true;

    [SerializeField]
    [HideLabel]
    [ShowIf("@m_type == InputListType.OnlyThese")]
    [PropertySpace(8, 8)]
    private BaseGameEvent[] m_onlyEvents = Array.Empty<BaseGameEvent>();

    [SerializeField]
    [HideLabel]
    [ShowIf("@m_type == InputListType.AnyButThese")]
    [PropertySpace(8, 8)]
    private BaseGameEvent[] m_anyButEvents = Array.Empty<BaseGameEvent>();

    [SerializeField]
    [EnumToggleButtons]
    [LabelWidth(100)]
    private InputType m_inputType;

    [SerializeField]
    [LabelWidth(100)]
    [EnumToggleButtons]
    private ComparisonOperation m_countType;

    private IEnumerable<BaseGameEvent> m_effectiveInputList;

    public override void Initialize()
    {
        base.Initialize();

        m_effectiveInputList = m_type switch
        {
            InputListType.AnyInput => m_usePhysicalInputs ? InputCountListenerSingleton.Instance.AllPhysicalInputEvents : InputCountListenerSingleton.Instance.AllInputEvents,
            InputListType.OnlyThese => m_onlyEvents,
            InputListType.AnyButThese => InputCountListenerSingleton.Instance.AllInputEvents.Except(m_anyButEvents),
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (BaseGameEvent inputEvent in m_effectiveInputList)
        {
            inputEvent.RemoveListener(OnInputTriggered);
            inputEvent.AddListener(OnInputTriggered);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        foreach (BaseGameEvent inputEvent in m_effectiveInputList)
        {
            inputEvent.RemoveListener(OnInputTriggered);
        }
    }

    private void OnInputTriggered()
    {
        // This is rather crado but it works?!
        // We need to delay the raise because the InputCountListener must have the callback first
        UniTask.Action(async () =>
        {
            await UniTask.WaitForEndOfFrame();
            RaiseOnPreconditionUpdated();
        }).Invoke();
    }

    public override bool Test()
    {
        float sinceTime = m_timeType == TimeType.TimeWindow ? Time.time - m_timeWindow :
            m_startTimeType == StartTimeType.GlobalVariable ? m_floatVariable.Value : (float)GraphBlackboard.Instance.Blackboard[m_blackboardKey];
        
        int inputCount = InputCountListenerSingleton.Instance.GetInputCount(m_effectiveInputList, sinceTime, m_inputType == InputType.Triggers);
        return m_countType switch
        {
            ComparisonOperation.Equal => inputCount == m_count,
            ComparisonOperation.NotEqual => inputCount != m_count,
            ComparisonOperation.StrictlyLowerThan => inputCount < m_count,
            ComparisonOperation.LowerOrEqualThan => inputCount <= m_count,
            ComparisonOperation.StrictlyGreaterThan => inputCount > m_count,
            ComparisonOperation.GreaterOrEqualThan => inputCount >= m_count,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
