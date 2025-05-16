using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class EntityStateSelector : SerializedMonoBehaviour
{
    [SerializeField] private IntVariable m_aze;
    
    [SerializeField][PropertySpace(0, 4)]
    private EntityState[] m_states;

    [SerializeField][BoxGroup][PropertySpace(4, 4)]
    private RotaryEncoderInputEvent m_rotaryEncoderEvent;

    [SerializeField][FoldoutGroup("Callbacks")]
    private UnityEvent<EntityState> m_onStateSet;

    [SerializeField][FoldoutGroup("Callbacks")]
    private UnityEvent<EntityState> m_onStateUnset;

    [SerializeField][FoldoutGroup("Callbacks")]
    private UnityEvent<EntityState, bool> m_onStateChanged;

    private int m_currentIndex;
    
    private void Start()
    {
        foreach (EntityState entityState in m_states)
        {
            entityState.SetValueWithoutNotify(false);
        }
        m_currentIndex = m_rotaryEncoderEvent.Index.Value;
        m_states[m_currentIndex].SetValueWithoutNotify(true);
        m_rotaryEncoderEvent.AddIndexListener(OnIndexChanged);
    }

    private void OnIndexChanged(int index)
    {
        Debug.Log($"OnIndexChanged current={m_currentIndex} ({m_states[m_currentIndex]}) next={index} ({m_states[index]})");
        m_states[m_currentIndex].Unset();
        m_onStateUnset?.Invoke(m_states[m_currentIndex]);
        m_onStateChanged?.Invoke(m_states[m_currentIndex], false);

        m_currentIndex = index;
        
        m_states[m_currentIndex].Set();
        m_onStateSet?.Invoke(m_states[m_currentIndex]);
        m_onStateChanged?.Invoke(m_states[m_currentIndex], true);
    }
}
