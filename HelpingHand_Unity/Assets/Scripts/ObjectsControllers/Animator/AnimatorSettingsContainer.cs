using System;

using UnityEngine;

[Serializable]
public class AnimatorSettingsContainer : IObjectSettingsContainer
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private string m_animatorVariableName = "Progress";

    private int m_progressVariableHash;

    public void Init()
    {
        m_progressVariableHash = Animator.StringToHash(m_animatorVariableName);
    }

    public void UpdateSettings(float progress)
    {
        if (m_animator == null)
        {
            return;
        }

        if (m_progressVariableHash == 0)
        {

        }

        m_animator.SetFloat(m_progressVariableHash, progress);
    }

    public void Dispose()
    {
    }
}
