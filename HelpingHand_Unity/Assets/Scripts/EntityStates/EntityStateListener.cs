using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class EntityStateListener : MonoBehaviour
{
    [SerializeField]
    [PropertySpace(0, 4)]
    [Required]
    private EntityState m_state;

    [SerializeField]
    [FoldoutGroup("Callbacks")]
    private UnityEvent m_onStateSet;

    [SerializeField]
    [FoldoutGroup("Callbacks")]
    private UnityEvent m_onStateUnset;

    [SerializeField]
    [FoldoutGroup("Callbacks")]
    private UnityEvent<bool> m_onStateChanged;

    private void OnEnable()
    {
        m_state.RemoveListener(OnValueChanged);
        m_state.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        m_state.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool value)
    {
        if (value)
        {
            m_onStateSet.Invoke();
        }
        else
        {
            m_onStateUnset.Invoke();
        }
        m_onStateChanged.Invoke(value);
    }
}
