using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class PreventDoublePerformedInteraction : IInputInteraction
{
    public float MinDelay = 0.05f;
    public float PressPoint = 0.5f;

    private float PressPointOrDefault => PressPoint > 0 ? PressPoint : InputSystem.settings.defaultButtonPressPoint;
    private float ReleasePointOrDefault => PressPointOrDefault * InputSystem.settings.buttonReleaseThreshold;
    private bool m_waitingForRelease;

    [NonSerialized] private float m_lastPerformedTime;

    public void Process(ref InputInteractionContext context)
    {
        var actuation = context.ComputeMagnitude();

        float now = Time.time;

        if (m_waitingForRelease)
        {
            if (actuation <= ReleasePointOrDefault && Time.time - m_lastPerformedTime > MinDelay)
            {
                m_waitingForRelease = false;
                if (Mathf.Approximately(0f, actuation))
                {
                    context.Canceled();
                }
                else
                {
                    context.Started();
                }
            }
        }
        else if (actuation >= PressPointOrDefault)
        {
            TryPerform(ref context);
        }
        else if (actuation > 0 && !context.isStarted)
        {
            context.Started();
        }
        else if (Mathf.Approximately(0f, actuation) && context.isStarted)
        {
            context.Canceled();
        }
    }

    private void TryPerform(ref InputInteractionContext context)
    {
        float now = Time.time;
        if (now - m_lastPerformedTime < MinDelay)
        {
            Debug.Log("Prevent double perform");
            return;
        }
        m_lastPerformedTime = now;
        m_waitingForRelease = true;
        context.PerformedAndStayPerformed();
    }

    public void Reset()
    {
    }
}
