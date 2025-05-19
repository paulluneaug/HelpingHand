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

    [SerializeField] [EnumToggleButtons] [LabelWidth(100)]
    private InputListType m_type;

    [SerializeField] [ShowIf("@m_type == InputListType.AnyInput")] [LabelWidth(100)] 
    private bool m_usePhysicalInputs = true;

    [SerializeField] [HideLabel] [ShowIf("@m_type == InputListType.OnlyThese")] 
    private BaseGameEvent[] m_onlyEvents = Array.Empty<BaseGameEvent>();
    
    [SerializeField] [HideLabel] [ShowIf("@m_type == InputListType.AnyButThese")] [LabelWidth(100)]
    private BaseGameEvent[] m_anyButEvents = Array.Empty<BaseGameEvent>();

    [SerializeField] [EnumToggleButtons] [LabelWidth(100)]
    private InputCountType m_countType;

    private bool m_isInputTriggered;
    private IEnumerable<BaseGameEvent> m_effectiveInputList; 
    
    public override void Initialize()
    {
        base.Initialize();

        m_effectiveInputList = m_type switch
        {
            InputListType.AnyInput => m_usePhysicalInputs ? InputCountListeneringleton.Instance.AllPhysicalInputEvents : InputCountListeneringleton.Instance.AllInputEvents,
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

    private void OnInputTriggered()
    {
        m_isInputTriggered = true;
        // This is rather crado but it works?!
        UniTask.Action(async () =>
        {
            await UniTask.WaitForEndOfFrame();
            RaiseOnPreconditionUpdated();
        }).Invoke();
    }

    public override bool Test()
    {
        int inputCount = InputCountListeneringleton.Instance.GetInputCount(m_effectiveInputList, m_timeWindow, m_countType == InputCountType.Triggers);

        // OnInputTriggered is called before InputCountSingleton.OnInputTriggered so the count is short of 1
        // If Test() is called from another condition, we don't need to add 1
        // if (inputCount == 0 && m_isInputTriggered)
        // {
        //     inputCount = 1;
        //     m_isInputTriggered = false;
        // }
        Debug.Log($"Input count = {inputCount}");
        return inputCount >= m_count;
    }

    public override void Dispose()
    {
        foreach (BaseGameEvent inputEvent in m_effectiveInputList)
        {
            inputEvent.RemoveListener(OnInputTriggered);
        }
    }
}
