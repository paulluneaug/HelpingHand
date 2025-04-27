using Events;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIToggleEventInvoker : MonoBehaviour
{
    [SerializeField]
    private BoolGameEvent m_event;
        
    private Toggle m_toggle;
    
    private void Awake()
    {
        m_toggle = GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        m_toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnDisable()
    {
        m_toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        m_event.Raise(isOn);
    }
}
