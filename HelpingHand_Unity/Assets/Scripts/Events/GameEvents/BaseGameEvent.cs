using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace Events
{
    public abstract class BaseGameEvent : SerializedScriptableObject, IGameEvent
    {
        [SerializeField]
        [ReadOnly]
        protected bool m_isActive = true;

        public virtual void Initialize() { }

        public bool IsActive
        {
            get => m_isActive;
            set
            {
                bool oldValue = m_isActive;
                m_isActive = value;
                if (m_isActive != oldValue)
                {
                    if (m_isActive)
                    {
                        OnActivate?.Invoke();
                    }
                    else
                    {
                        OnDeactivate?.Invoke();
                    }
                }
            }
        }

        public event Action OnActivate;
        public event Action OnDeactivate;
        public event Action OnEventRaised;

        private void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }

        private readonly List<IGameEventListener> m_listeners = new();
        private readonly List<Action> m_actions = new();

        public virtual void Raise()
        {
            RaiseEvent();

            if (!IsActive)
            {
                return;
            }

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
                _ = m_listeners.Remove(listener);
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
                _ = m_actions.Remove(action);
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
            Raise();

            if (!IsActive)
            {
                return;
            }

            for (var i = m_typedListeners.Count - 1; i >= 0; i--)
            {
                m_typedListeners[i].OnEventRaised(value);
            }

            for (var i = m_typedActions.Count - 1; i >= 0; i--)
            {
                m_typedActions[i](value);
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
                _ = m_typedListeners.Remove(listener);
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
                _ = m_typedActions.Remove(action);
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