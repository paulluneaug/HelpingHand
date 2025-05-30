using Sirenix.OdinInspector;

using UnityEngine;

[System.Serializable]
public class ConditionBoolVariable : ConditionBase
{
    [SerializeField]
    [LabelWidth(100)]
    [InlineEditor]
    private BoolVariable m_variable;

    [SerializeField]
    [LabelWidth(100)]
    private bool m_value;

    public override bool Test()
    {
        return m_value == m_variable.Value;
    }

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
}
