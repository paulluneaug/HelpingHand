using System;

using Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Events/Button")]
public class ButtonInputEvent : BaseGameEvent
{
    [SerializeField]
    private GameEvent m_buttonDownEvent;

    [SerializeField]
    private GameEvent m_buttonUpEvent;

    [SerializeField]
    private GameEvent m_buttonPressedEvent;

#if UNITY_EDITOR
    private void Awake()
    {
        if (m_buttonDownEvent == null)
        {
            m_buttonDownEvent = ScriptableObject.CreateInstance<GameEvent>();
            m_buttonDownEvent.name = "OnDownEvent";
            AssetDatabase.AddObjectToAsset(m_buttonDownEvent, this);
            m_buttonUpEvent = ScriptableObject.CreateInstance<GameEvent>();
            m_buttonUpEvent.name = "OnUpEvent";
            AssetDatabase.AddObjectToAsset(m_buttonUpEvent, this);
            m_buttonPressedEvent = ScriptableObject.CreateInstance<GameEvent>();
            m_buttonPressedEvent.name = "OnPressedEvent";
            AssetDatabase.AddObjectToAsset(m_buttonPressedEvent, this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }
#endif

    public void RaiseDown()
    {
        m_buttonDownEvent.Raise();
    }

    public void RaiseUp()
    {
        m_buttonUpEvent.Raise();
    }

    public void RaisePressed()
    {
        m_buttonPressedEvent.Raise();
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

    public void AddPressedListener(GameEventListener listener)
    {
        m_buttonPressedEvent.AddListener(listener);
    }

    public void AddPressedListener(Action action)
    {
        m_buttonPressedEvent.AddListener(action);
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

    public void RemovePressedListener(GameEventListener listener)
    {
        m_buttonPressedEvent.RemoveListener(listener);
    }

    public void RemovePressedListener(Action action)
    {
        m_buttonPressedEvent.RemoveListener(action);
    }
}
