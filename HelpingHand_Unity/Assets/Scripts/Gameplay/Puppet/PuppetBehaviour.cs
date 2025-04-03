using System;
using System.Collections.Generic;

using UnityEngine;

public class PuppetBehaviour : MonoBehaviour
{
    [SerializeReference] private List<PuppetAction> m_actions;


    [NonSerialized] private Puppet m_puppet;
    [NonSerialized] private int m_currentAction;

    [NonSerialized] private bool m_finished;


    public void StartBehaviour(Puppet puppet)
    {
        m_puppet = puppet;
        m_currentAction = 0;
        m_finished = false;


        PuppetAction action = m_actions[m_currentAction];
        action.StartAction(puppet);
    }

    public void UpdateBehaviour(float deltaTime)
    {
        if (m_finished)
        {
            return;
        }

        PuppetAction action = m_actions[m_currentAction];
        if (action.IsFinished)
        {
            action.EndAction();

            ++m_currentAction;
            if (m_currentAction >= m_actions.Count)
            {
                FinishBheviour();
                return;
            }
            PuppetAction nextAction = m_actions[m_currentAction];
            nextAction.StartAction(m_puppet);
        }

        action.UpdateAction(deltaTime);
    }

    private void FinishBheviour()
    {
        m_finished = true;
    }

}
