using System;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Extensions;

[Serializable]
public class ConditionIntValue : ConditionBase
{
    private enum ValueType
    {
        [LabelText("Global variable")] GlobalVariable,
        [LabelText("Blackboard")] Blackboard
    }

    [SerializeField]
    [EnumToggleButtons]
    private ValueType m_valueType;
    
    [SerializeField]
    [ShowIf("@m_valueType == ValueType.GlobalVariable")]
    [LabelWidth(100)]
    [InlineEditor]
    private IntVariable m_variable;

    [SerializeField]
    [ShowIf("@m_valueType == ValueType.Blackboard")]
    [LabelWidth(100)]
    private string m_key;
    
    [SerializeField]
    [LabelWidth(100)]
    private ComparisonOperation m_comparison;

    [SerializeField]
    [LabelWidth(100)]
    [ShowIf("@m_comparison == ComparisonOperation.Equal || m_comparison == ComparisonOperation.NotEqual")]
    private bool m_useBounds;

    [SerializeField]
    [LabelWidth(100)]
    [ShowIf("@m_useBounds && (m_comparison == ComparisonOperation.Equal || m_comparison == ComparisonOperation.NotEqual)")]
    private bool m_boundsInclusive = true;

    [SerializeField]
    [LabelWidth(100)]
    [HideIf("@m_useBounds")]
    private int m_value;

    [SerializeField]
    [LabelWidth(100)]
    [ShowIf("@m_useBounds")]
    [HorizontalGroup("Range")]
    private int m_valueMin;

    [SerializeField]
    [LabelWidth(100)]
    [ShowIf("@m_useBounds")]
    [HorizontalGroup("Range")]
    private int m_valueMax;

    public override void Initialize()
    {
        base.Initialize();
        m_variable?.RemoveListener(RaiseOnPreconditionUpdated);
        m_variable?.AddListener(RaiseOnPreconditionUpdated);
    }

    public override void Dispose()
    {
        base.Dispose();
        m_variable.RemoveListener(RaiseOnPreconditionUpdated);
    }

    public override bool Test()
    {
        return m_comparison switch
        {
            ComparisonOperation.Equal => CompareEquals(),
            ComparisonOperation.NotEqual => !CompareEquals(),
            ComparisonOperation.StrictlyLowerThan => GetValue() < m_value,
            ComparisonOperation.LowerOrEqualThan => GetValue() <= m_value,
            ComparisonOperation.StrictlyGreaterThan => GetValue() > m_value,
            ComparisonOperation.GreaterOrEqualThan => GetValue() >= m_value,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool CompareEquals()
    {
        if (m_useBounds)
        {
            return GetValue().Between(m_valueMin, m_valueMax, m_boundsInclusive);
        }
        else
        {
            return m_value == GetValue();
        }
    }

    private int GetValue()
    {
        if (m_valueType == ValueType.GlobalVariable)
        {
            return m_variable.Value;
        } 
        else if (m_valueType == ValueType.Blackboard)
        {
            if (GraphBlackboard.Instance.TryGetValue(m_key, out int value))
            {
                return value;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"No value with key {m_key} in blackboard");
            }
        }

        throw new ArgumentOutOfRangeException();
    }
}