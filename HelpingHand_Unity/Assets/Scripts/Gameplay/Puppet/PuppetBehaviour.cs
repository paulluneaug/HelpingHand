using System;

using UnityEngine;

using UnityUtility.Extensions;
using UnityUtility.Timer;

public class PuppetBehaviour : MonoBehaviour
{
    private enum PuppetAction : int
    {
        Wait = 0,
        Walk = 1,
        TurnRight = 2,
        TurnLeft = 3,
    }

    [SerializeField] private PuppetAction[] m_actions;

    [NonSerialized]
    private readonly PuppetState[] m_puppetStates = new PuppetState[]
    {
        new WaitState(),
        new WalkState(),
        new RotateRightState(),
        new RotateLeftState(),
    };

    [NonSerialized] private Puppet m_puppet;
    [NonSerialized] private PuppetState m_currentState;
    [NonSerialized] private int m_currentActionIndex;

    [NonSerialized] private bool m_finished;

    [NonSerialized] private Timer m_actionsTimer;


    public void StartBehaviour(Puppet puppet)
    {
        m_puppet = puppet;
        m_finished = false;

        m_actionsTimer = new Timer(puppet.Settings.ActionDuration, true);
        m_actionsTimer.Start();

        m_puppetStates.ForEach(state => state.InitState(puppet));

        m_currentActionIndex = -1;
        StartNextAction();
    }

    public void UpdateBehaviour(float deltaTime)
    {
        if (m_actionsTimer.Update(deltaTime))
        {
            StartNextAction();
        }


        m_currentState.UpdateState(m_actionsTimer.Progress, deltaTime);
    }

    private void StartNextAction()
    {
        m_currentState?.EndState();
        m_currentActionIndex = (m_currentActionIndex + 1) % m_actions.Length;

        m_currentState = m_puppetStates[(int)m_actions[m_currentActionIndex]];
        m_currentState.BeginState();
    }
}
