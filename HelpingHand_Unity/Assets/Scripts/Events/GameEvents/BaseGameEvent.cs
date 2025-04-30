using System;
using System.Collections.Generic;

using UnityEngine;

namespace Events
{
    public abstract class BaseGameEvent : ScriptableObject, IGameEvent
    {
        protected readonly List<IGameEventListener> m_listeners = new();
        protected readonly List<Action> m_actions = new();

        public virtual void Raise()
        {
            for (var i = m_listeners.Count - 1; i >= 0; i--)
            {
                m_listeners[i].OnEventRaised();
            }

            for (var i = m_actions.Count - 1; i >= 0; i--)
            {
                m_actions[i]();
            }
        }

        public void AddListener(IGameEventListener listener)
        {
            if (!m_listeners.Contains(listener))
            {
                m_listeners.Add(listener);
            }
        }

        public void RemoveListener(IGameEventListener listener)
        {
            if (m_listeners.Contains(listener))
            {
                m_listeners.Remove(listener);
            }
        }

        public void AddListener(Action action)
        {
            if (!m_actions.Contains(action))
            {
                m_actions.Add(action);
            }
        }

        public void RemoveListener(Action action)
        {
            if (m_actions.Contains(action))
            {
                m_actions.Remove(action);
            }
        }

        public virtual void RemoveAll()
        {
            m_listeners.Clear();
            m_actions.Clear();
        }
    }

    public abstract class BaseGameEvent<T> : BaseGameEvent, IGameEvent<T>
    {
        private readonly List<IGameEventListener<T>> m_typedListeners = new();
        private readonly List<Action<T>> m_typedActions = new();

        public void Raise(T value)
        {
            for (var i = m_typedListeners.Count - 1; i >= 0; i--)
            {
                m_typedListeners[i].OnEventRaised(value);
            }

            for (var i = m_listeners.Count - 1; i >= 0; i--)
            {
                m_listeners[i].OnEventRaised();
            }

            for (var i = m_typedActions.Count - 1; i >= 0; i--)
            {
                m_typedActions[i](value);
            }

            for (var i = m_actions.Count - 1; i >= 0; i--)
            {
                m_actions[i]();
            }
        }

        public void AddListener(IGameEventListener<T> listener)
        {
            if (!m_typedListeners.Contains(listener))
            {
                m_typedListeners.Add(listener);
            }
        }

        public void RemoveListener(IGameEventListener<T> listener)
        {
            if (m_typedListeners.Contains(listener))
            {
                m_typedListeners.Remove(listener);
            }
        }

        public void AddListener(Action<T> action)
        {
            if (!m_typedActions.Contains(action))
            {
                m_typedActions.Add(action);
            }
        }

        public void RemoveListener(Action<T> action)
        {
            if (m_typedActions.Contains(action))
            {
                m_typedActions.Remove(action);
            }
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
            m_typedListeners.Clear();
            m_typedActions.Clear();
        }
    }
}