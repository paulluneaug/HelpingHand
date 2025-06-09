using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public abstract class BaseGameEventListener<TEvent> : MonoBehaviour, IGameEventListener where TEvent : BaseGameEvent
    {
        protected TEvent GameEvent => m_event;
        protected UnityEvent Response => m_response;

        [SerializeField]
        private bool m_doUnregisterOnDisable = true;

        [SerializeField]
        private TEvent m_event;

        [SerializeField]
        private UnityEvent m_response;

        private TEvent m_previouslyRegisteredEvent;

        public void OnEventRaised()
        {
            m_response.Invoke();
        }

        private void OnEnable()
        {
            if (m_event != null)
            {
                Register();
            }
        }

        private void OnDisable()
        {
            if (!m_doUnregisterOnDisable)
            {
                return;
            }

            m_event?.RemoveListener(this);
        }

        private void Register()
        {
            m_previouslyRegisteredEvent?.RemoveListener(this);

            m_event.AddListener(this);
            m_previouslyRegisteredEvent = m_event;
        }
    }

    public abstract class BaseGameEventListener<TType, TEvent> : MonoBehaviour, IGameEventListener<TType> where TEvent : BaseGameEvent<TType>
    {
        protected TEvent GameEvent => m_event;
        protected UnityEvent<TType> Response => m_response;

        [SerializeField]
        private bool m_doUnregisterOnDisable = true;

        [SerializeField]
        private TEvent m_event;

        [SerializeField]
        private UnityEvent<TType> m_response;

        private TEvent m_previouslyRegisteredEvent;

        public void OnEventRaised(TType value)
        {
            m_response.Invoke(value);
        }

        private void OnEnable()
        {
            if (m_event != null)
            {
                Register();
            }
        }

        private void OnDisable()
        {
            if (!m_doUnregisterOnDisable)
            {
                return;
            }

            m_event?.RemoveListener(this);
        }

        private void Register()
        {
            m_previouslyRegisteredEvent?.RemoveListener(this);

            m_event.AddListener(this);
            m_previouslyRegisteredEvent = m_event;
        }
    }
}