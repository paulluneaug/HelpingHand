using System;

using UnityEngine;
using UnityEngine.Rendering;

using UnityUtility.MathU;
using UnityUtility.Timer;

[Serializable]
public class RotateAction : PuppetAction
{
    private enum RotationBehaviour
    {
        Left,
        UTurn,
        Right,
    }

    [SerializeField] private RotationBehaviour m_rotationBehaviour;
    [SerializeField, Min(0.1f)] private float m_rotationSpeedMultiplier = 1.0f;

    [NonSerialized] private Timer m_turnTimer;
    [NonSerialized] private float m_targetAngle;
    [NonSerialized] private float m_rotationSpeed;

    public override void StartAction(Puppet puppet)
    {
        base.StartAction(puppet);

        float angle = m_rotationBehaviour switch
        {
            RotationBehaviour.Left => -MathUf.PI / 2.0f,
            RotationBehaviour.UTurn => -MathUf.PI,
            RotationBehaviour.Right => MathUf.PI / 2.0f,
            _ => throw new ArgumentOutOfRangeException(),
        };
        m_rotationSpeed = puppet.Settings.PuppetRotationSpeed * m_rotationSpeedMultiplier * MathUf.TAU;
        float rotationTime = MathUf.Abs(angle) / m_rotationSpeed;
        m_turnTimer = new Timer(rotationTime, false);
        m_turnTimer.Start();

        m_targetAngle = puppet.transform.rotation.eulerAngles.y * MathUf.DEG_2_RAD + angle;
    }

    public override void UpdateAction(float deltaTime)
    {
        if (m_finished)
        {
            return;
        }
        base.UpdateAction(deltaTime);

        if (m_turnTimer.Update(deltaTime))
        {
            FinishAction();
            return;
        }

        float angleSign = m_rotationBehaviour switch
        {
            RotationBehaviour.Left => -1,
            RotationBehaviour.UTurn => -1,
            RotationBehaviour.Right => 1,
            _ => throw new ArgumentOutOfRangeException(),
        };

        float angle = deltaTime * angleSign * m_rotationSpeed;

        m_puppet.Rotate(angle);
    }

    public override void EndAction()
    {
        base.EndAction();
        m_puppet.SetRootationY(m_targetAngle);
    }
}
