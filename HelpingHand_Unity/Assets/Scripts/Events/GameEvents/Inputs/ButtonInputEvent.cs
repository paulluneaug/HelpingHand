using System;

using Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inputs/Button")]
public class ButtonInputEvent : BaseGameEvent
{
    [SerializeField]
    private GameEvent m_buttonDownEvent;
    
    [SerializeField]
    private GameEvent m_buttonUpEvent;

    [SerializeField] 
    private BoolVariable m_buttonState;

    public BoolVariable ButtonState => m_buttonState;

#if UNITY_EDITOR
    private void Awake()
    {
        if (m_buttonDownEvent == null)
        {
            m_buttonDownEvent = ScriptableObject.CreateInstance<GameEvent>();
            m_buttonDownEvent.name = $"{name}_OnDownEvent";
            AssetDatabase.AddObjectToAsset(m_buttonDownEvent, this);
            m_buttonUpEvent = ScriptableObject.CreateInstance<GameEvent>();
            m_buttonUpEvent.name = $"{name}_OnUpEvent";
            AssetDatabase.AddObjectToAsset(m_buttonUpEvent, this);
            m_buttonState = ScriptableObject.CreateInstance<BoolVariable>();
            m_buttonState.name = $"{name}_State";
            AssetDatabase.AddObjectToAsset(m_buttonState, this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }
#endif

    public void RaiseDown()
    {
        m_buttonDownEvent.Raise();
        Raise();
    }

    public void RaiseUp()
    {
        m_buttonUpEvent.Raise();
    }

    public void AddDownListener(GameEventListener listener)
    {
        m_buttonDownEvent.AddListener(listener);
    }

    public void AddDownListener(Action action)
    {
        m_buttonDownEvent.AddListener(action);
    }

    public void AddUpListener(GameEventListener listener)
    {
        m_buttonUpEvent.AddListener(listener);
    }

    public void AddUpListener(Action action)
    {
        m_buttonUpEvent.AddListener(action);
    }

    public void RemoveDownListener(GameEventListener listener)
    {
        m_buttonDownEvent.RemoveListener(listener);
    }

    public void RemoveDownListener(Action action)
    {
        m_buttonDownEvent.RemoveListener(action);
    }

    public void RemoveUpListener(GameEventListener listener)
    {
        m_buttonUpEvent.RemoveListener(listener);
    }

    public void RemoveUpListener(Action action)
    {
        m_buttonUpEvent.RemoveListener(action);
    }
}
