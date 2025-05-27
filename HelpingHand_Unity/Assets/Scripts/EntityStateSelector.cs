using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class EntityStateSelector : SerializedMonoBehaviour
{
    [SerializeField] private IntVariable m_rotaryEncoderIndex;

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
        m_currentIndex = m_rotaryEncoderEvent.Index.Value.Mod(m_states.Length);
        m_states[m_currentIndex].SetValueWithoutNotify(true);
        m_rotaryEncoderEvent.AddIndexListener(OnIndexChanged);
    }

    private void OnIndexChanged(int index)
    {
        m_states[m_currentIndex].Unset();
        m_onStateUnset?.Invoke(m_states[m_currentIndex]);
        m_onStateChanged?.Invoke(m_states[m_currentIndex], false);

        m_currentIndex = index.Mod(m_states.Length);

        m_states[m_currentIndex].Set();
        m_onStateSet?.Invoke(m_states[m_currentIndex]);
        m_onStateChanged?.Invoke(m_states[m_currentIndex], true);
    }
}
