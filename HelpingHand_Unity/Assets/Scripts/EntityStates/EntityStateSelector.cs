using System;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class EntityStateSelector : SerializedMonoBehaviour
{
    [SerializeField]
    [PropertySpace(0, 4)]
    private EntityState[] m_states;

    [SerializeField]
    private int m_selectedIndex;

    [SerializeField]
    [BoxGroup]
    [PropertySpace(4, 4)]
    private RotaryEncoderInputEvent m_rotaryEncoderEvent;

    [SerializeField]
    [FoldoutGroup("Callbacks")]
    private UnityEvent<EntityState> m_onStateSet;

    [SerializeField]
    [FoldoutGroup("Callbacks")]
    private UnityEvent<EntityState> m_onStateUnset;

    [SerializeField]
    [FoldoutGroup("Callbacks")]
    private UnityEvent<EntityState, bool> m_onStateChanged;

    private int m_currentIndex;

    private void Awake()
    {
        m_currentIndex = m_selectedIndex;
    }

    private void Start()
    {
        foreach (EntityState entityState in m_states)
        {
            entityState.SetValueWithoutNotify(false);
        }
        m_currentIndex = m_selectedIndex;
        if (m_rotaryEncoderEvent.IsActive)
        {
            m_states[m_currentIndex].Set();
            m_rotaryEncoderEvent.AddStepLeftListener(OnStepLeft);
            m_rotaryEncoderEvent.AddStepRightListener(OnStepRight);
        }

        m_rotaryEncoderEvent.OnActivate += OnEventActivate;
        m_rotaryEncoderEvent.OnDeactivate += OnEventDeactivate;
    }

    private void OnDestroy()
    {
        m_rotaryEncoderEvent.OnActivate -= OnEventActivate;
        m_rotaryEncoderEvent.OnDeactivate -= OnEventDeactivate;
    }

    private void OnEventDeactivate()
    {
        m_rotaryEncoderEvent.RemoveStepLeftListener(OnStepLeft);
        m_rotaryEncoderEvent.RemoveStepRightListener(OnStepRight);
    }

    private void OnEventActivate()
    {
        m_rotaryEncoderEvent.AddStepLeftListener(OnStepLeft);
        m_rotaryEncoderEvent.AddStepRightListener(OnStepRight);
    }

    private void OnStepRight()
    {
        int newIndex = (m_currentIndex + 1).Mod(m_states.Length);
        OnIndexChanged(newIndex);
    }

    private void OnStepLeft()
    {
        int newIndex = (m_currentIndex - 1).Mod(m_states.Length);
        OnIndexChanged(newIndex);
    }

    private void OnIndexChanged(int newIndex)
    {
        m_states[m_currentIndex].Unset();
        m_onStateUnset?.Invoke(m_states[m_currentIndex]);
        m_onStateChanged?.Invoke(m_states[m_currentIndex], false);

        m_currentIndex = newIndex;

        m_states[m_currentIndex].Set();
        m_onStateSet?.Invoke(m_states[m_currentIndex]);
        m_onStateChanged?.Invoke(m_states[m_currentIndex], true);
    }
}
