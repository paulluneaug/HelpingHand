using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.SerializedDictionary;

using static SimonSequence;

[Serializable]
public class SimonManager
{
    [SerializeField] private SerializedDictionary<SimonColor, BaseVariable<bool>> m_simonActions;

    [NonSerialized] private readonly Queue<SimonColor> m_inputQueue = new Queue<SimonColor>();

    [NonSerialized] private bool m_listenForInputs = false;
    [NonSerialized] private Action<bool>[] m_eventActions;

    [NonSerialized] private UniTask<bool>? m_currentSimonTask = null;
    [NonSerialized] private bool m_cancelTask;

    public void Initialize()
    {
        m_listenForInputs = false;

        m_eventActions = new Action<bool>[m_simonActions.Count];

        int actionIndex = 0;
        foreach (KeyValuePair<SimonColor, BaseVariable<bool>> pair in m_simonActions)
        {
            SimonColor color = pair.Key;
            m_eventActions[actionIndex] = (bool inputValue) => OnSimonInputPerformed(inputValue, color);
            pair.Value.AddListener(m_eventActions[actionIndex++]);
        }


    }

    public void Dispose()
    {
        int actionIndex = 0;
        foreach (KeyValuePair<SimonColor, BaseVariable<bool>> pair in m_simonActions)
        {
            pair.Value.AddListener(m_eventActions[actionIndex++]);
        }
    }

    public async UniTask<bool> StartSequence(SimonSequence sequence)
    {
        m_cancelTask = false;
        m_currentSimonTask = SimonSequence(sequence);
        bool result = await m_currentSimonTask.Value;

        m_listenForInputs = false;

        return result;
    }

    public async UniTask<bool> ResumeSequence()
    {
        bool result = await m_currentSimonTask.Value;
        m_listenForInputs = false;
        return result;
    }

    public void CancelSequence()
    {
        m_cancelTask = true;
        m_currentSimonTask = null;
    }

    private async UniTask<bool> SimonSequence(SimonSequence sequence)
    {
        m_inputQueue.Clear();

        GameManager.Instance.ArduinoConnectorManager.SendSimonSequence(sequence);

        m_listenForInputs = true;

        int sequenceIndex = 0;

        while (sequenceIndex < sequence.Sequence.Length && !m_cancelTask)
        {
            while (m_inputQueue.Count == 0 && !m_cancelTask)
            {
                await UniTask.Yield();
            }

            if (m_cancelTask)
            {
                return false;
            }

            SimonColor performedInput = m_inputQueue.Dequeue();
            if (performedInput != sequence.Sequence[sequenceIndex++])
            {
                return false;
            }
        }
        return !m_cancelTask;
    }



    private void OnSimonInputPerformed(bool inputValue, SimonColor color)
    {
        if (!inputValue)
        {
            return;
        }
        if (!m_listenForInputs)
        {
            return;
        }

        m_inputQueue.Enqueue(color);
    }
}
