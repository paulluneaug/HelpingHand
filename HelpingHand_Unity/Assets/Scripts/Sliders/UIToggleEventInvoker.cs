using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIToggleEventInvoker : MonoBehaviour
{
    [SerializeField]
    private BoolVariable m_variable;

    private Toggle m_toggle;

    private void Awake()
    {
        m_toggle = GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        m_variable.Value = m_toggle.isOn;
        m_toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnDisable()
    {
        m_toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        m_variable.Value = isOn;
    }
}
