using System;
using System.Collections.Generic;

using UnityEngine;

public class PuppetBehaviour : MonoBehaviour
{
    private enum PuppetAction
    {
        Wait,
        MoveForward,
        TurnRight,
        TurnLeft,
    }

    [SerializeField] private List<PuppetAction> m_actions;


    [NonSerialized] private Puppet m_puppet;
    [NonSerialized] private int m_currentAction;

    [NonSerialized] private bool m_finished;


    public void StartBehaviour(Puppet puppet)
    {
        m_puppet = puppet;
        m_currentAction = 0;
        m_finished = false;


        //PuppetState action = m_actions[m_currentAction];
        //action.BeginState(puppet);
    }

    public void UpdateBehaviour(float deltaTime)
    {
        if (m_finished)
        {
            return;
        }

        //PuppetState action = m_actions[m_currentAction];
        //if (action.IsFinished)
        //{
        //    action.EndState();

        //    ++m_currentAction;
        //    if (m_currentAction >= m_actions.Count)
        //    {
        //        FinishBheviour();
        //        return;
        //    }
        //    PuppetState nextAction = m_actions[m_currentAction];
        //    nextAction.BeginState(m_puppet);
        //}

        //action.UpdateState(deltaTime);
    }

    private void FinishBheviour()
    {
        m_finished = true;
    }

}
