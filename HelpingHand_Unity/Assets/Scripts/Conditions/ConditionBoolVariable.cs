using UnityEngine;

[System.Serializable]
public class ConditionBoolVariable : ConditionBase
{
    [SerializeField]
    private BoolVariable m_variable;

    public override bool Test()
    {
        return m_variable.Value;
    }

    public override void Initialize()
    {
        base.Initialize();
        m_variable.RemoveListener(RaiseOnPreconditionUpdated);
        m_variable.AddListener(RaiseOnPreconditionUpdated);
    }
}
