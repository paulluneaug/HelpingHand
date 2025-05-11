using System;

using UnityEngine;

[Serializable]
public class ToggleInputTrigger : InputTrigger
{
    [SerializeField]
    private BoolVariable m_toggleVariable;

    [SerializeField]
    private bool m_triggerIfTrue = true;
    
    [SerializeField]
    private float m_timeToTrigger = 0.05f;

    [SerializeField]
    private bool m_isImmediate = false;

    public override bool IsRaised => m_isImmediate ? m_isRaised : m_triggerIfTrue == m_toggleVariable.Value;

    public override void Initialize()
    {
        base.Initialize();
        SetActive(true);
    }

    protected override void Activate()
    {
        m_toggleVariable.AddListener(OnToggleValueChanged);
    }

    protected override void Deactivate()
    {
        m_toggleVariable.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (m_triggerIfTrue ==  m_toggleVariable.Value)
        {
            m_isRaised = true;
            RaiseTrigger();
            
            DialogueManager.Instance.StartCoroutine(ReactivateCoroutine());
        }
    }
}