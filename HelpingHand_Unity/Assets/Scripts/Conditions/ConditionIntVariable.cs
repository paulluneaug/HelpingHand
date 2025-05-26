using System;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Extensions;

[System.Serializable]
public class ConditionIntVariable : ConditionBase
{
    [SerializeField]
    [LabelWidth(100)]
    [InlineEditor]
    private IntVariable m_variable;

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
        m_variable.RemoveListener(RaiseOnPreconditionUpdated);
        m_variable.AddListener(RaiseOnPreconditionUpdated);
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
            ComparisonOperation.StrictlyLowerThan => m_variable.Value < m_value,
            ComparisonOperation.LowerOrEqualThan => m_variable.Value <= m_value,
            ComparisonOperation.StrictlyGreaterThan => m_variable.Value > m_value,
            ComparisonOperation.GreaterOrEqualThan => m_variable.Value >= m_value,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool CompareEquals()
    {
        if (m_useBounds)
        {
            return m_variable.Value.Between(m_valueMin, m_valueMax, m_boundsInclusive);
        }
        else
        {
            return m_value == m_variable.Value;
        }
    }
}