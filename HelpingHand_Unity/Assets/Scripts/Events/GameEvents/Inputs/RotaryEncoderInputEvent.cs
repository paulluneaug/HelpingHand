using System;

using Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inputs/Rotary Encoder")]
public class RotaryEncoderInputEvent : BaseGameEvent
{
    public IntVariable Index => m_indexVariable;
    public GameEvent StepLeftEvent => m_stepLeftEvent;
    public GameEvent StepRightEvent => m_stepRightEvent;


    [SerializeField]
    private IntVariable m_indexVariable;

    [SerializeField]
    private GameEvent m_stepLeftEvent;

    [SerializeField]
    private GameEvent m_stepRightEvent;

#if UNITY_EDITOR
    private void Awake()
    {
        if (m_indexVariable == null)
        {
            m_indexVariable = ScriptableObject.CreateInstance<IntVariable>();
            m_indexVariable.name = $"{name}_Variable_Index";
            AssetDatabase.AddObjectToAsset(m_indexVariable, this);
            m_stepLeftEvent = ScriptableObject.CreateInstance<GameEvent>();
            m_stepLeftEvent.name = $"{name}_OnStepLeftEvent";
            AssetDatabase.AddObjectToAsset(m_stepLeftEvent, this);
            m_stepRightEvent = ScriptableObject.CreateInstance<GameEvent>();
            m_stepRightEvent.name = $"{name}_OnStepRightEvent";
            AssetDatabase.AddObjectToAsset(m_stepRightEvent, this);
        }
    }
#endif

    public void SetIndex(int index)
    {
        m_indexVariable.Value = index;
    }

    public void RaiseStepLeft()
    {
        m_stepLeftEvent.Raise();
        Raise();
    }

    public void RaiseStepRight()
    {
        m_stepRightEvent.Raise();
        Raise();
    }

    public void AddIndexListener(IntGameEventListener listener)
    {
        m_indexVariable.AddListener(listener);
    }

    public void AddIndexListener(GameEventListener listener)
    {
        m_indexVariable.AddListener(listener);
    }

    public void AddIndexListener(Action<int> listener)
    {
        m_indexVariable.AddListener(listener);
    }

    public void AddIndexListener(Action listener)
    {
        m_indexVariable.AddListener(listener);
    }

    public void AddStepLeftListener(GameEventListener listener)
    {
        m_stepLeftEvent.AddListener(listener);
    }

    public void AddStepLeftListener(Action action)
    {
        m_stepLeftEvent.AddListener(action);
    }

    public void AddStepRightListener(GameEventListener listener)
    {
        m_stepRightEvent.AddListener(listener);
    }

    public void AddStepRightListener(Action action)
    {
        m_stepRightEvent.AddListener(action);
    }

    public void RemoveIndexListener(IntGameEventListener listener)
    {
        m_indexVariable.RemoveListener(listener);
    }

    public void RemoveIndexListener(GameEventListener listener)
    {
        m_indexVariable.RemoveListener(listener);
    }

    public void RemoveIndexListener(Action<int> listener)
    {
        m_indexVariable.RemoveListener(listener);
    }

    public void RemoveIndexListener(Action listener)
    {
        m_indexVariable.RemoveListener(listener);
    }

    public void RemoveStepLeftListener(GameEventListener listener)
    {
        m_stepLeftEvent.RemoveListener(listener);
    }

    public void RemoveStepLeftListener(Action action)
    {
        m_stepLeftEvent.RemoveListener(action);
    }

    public void RemoveStepRightListener(GameEventListener listener)
    {
        m_stepRightEvent.RemoveListener(listener);
    }

    public void RemoveStepRightListener(Action action)
    {
        m_stepRightEvent.RemoveListener(action);
    }
}
