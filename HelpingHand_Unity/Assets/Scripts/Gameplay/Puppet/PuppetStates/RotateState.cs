using System;

using UnityUtility.MathU;

public abstract class RotateState : PuppetState
{
    [NonSerialized] protected float m_direction;

    [NonSerialized] private float m_startRotation;
    [NonSerialized] private float m_endRotation;

    public override void BeginState()
    {
        base.BeginState();

        m_startRotation = m_puppet.transform.rotation.eulerAngles.y * MathUf.DEG_2_RAD;
        m_endRotation = m_startRotation + m_direction * MathUf.PI / 2.0f;
    }

    public override void UpdateState(float progress, float deltaTime)
    {
        base.UpdateState(progress, deltaTime);
        m_puppet.SetRotationY(MathUf.Lerp(m_startRotation, m_endRotation, progress));
    }

    public override void EndState()
    {
        m_puppet.SetRotationY(m_endRotation);
    }
}
