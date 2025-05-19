using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Events;

using Sirenix.OdinInspector;

using UnityEngine;

[System.Serializable]
public class ConditionInputCount : ConditionBase
{
    private enum InputListType
    {
        [LabelText("Any input")] AnyInput,
        [LabelText("Only these inputs")] OnlyThese,
        [LabelText("Any but these inputs")] AnyButThese,
    }

    private enum InputCountType
    {
        [LabelText("Triggers count")] Triggers,
        [LabelText("Inputs count")] Inputs,
    }
    
    [SerializeField] [LabelWidth(100)]
    private int m_count;

    [SerializeField] [LabelWidth(100)]
    private float m_timeWindow;

    [SerializeField] [EnumToggleButtons] [LabelWidth(100)] [OnValueChanged("OnTypeChanged")]
    private InputListType m_type;
    
    private void OnTypeChanged()
    {
        m_onlyEvents = Array.Empty<BaseGameEvent>();
        m_anyButEvents = Array.Empty<BaseGameEvent>();
    }

    [SerializeField] [ShowIf("@m_type == InputListType.AnyInput")] [LabelWidth(100)] 
    private bool m_usePhysicalInputs = true;

    [SerializeField] [HideLabel] [ShowIf("@m_type == InputListType.OnlyThese")] [PropertySpace(8, 8)]
    private BaseGameEvent[] m_onlyEvents = Array.Empty<BaseGameEvent>();
    
    [SerializeField] [HideLabel] [ShowIf("@m_type == InputListType.AnyButThese")] [PropertySpace(8, 8)]
    private BaseGameEvent[] m_anyButEvents = Array.Empty<BaseGameEvent>();

    [SerializeField] [EnumToggleButtons] [LabelWidth(100)]
    private InputCountType m_countType;

    private IEnumerable<BaseGameEvent> m_effectiveInputList; 
    
    public override void Initialize()
    {
        base.Initialize();

        m_effectiveInputList = m_type switch
        {
            InputListType.AnyInput => m_usePhysicalInputs ? InputCountListenerSingleton.Instance.AllPhysicalInputEvents : InputCountListenerSingleton.Instance.AllInputEvents,
            InputListType.OnlyThese => m_onlyEvents,
            InputListType.AnyButThese => m_anyButEvents,
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
        int inputCount = InputCountListenerSingleton.Instance.GetInputCount(m_effectiveInputList, m_timeWindow, m_countType == InputCountType.Triggers);
        return inputCount >= m_count;
    }
}
