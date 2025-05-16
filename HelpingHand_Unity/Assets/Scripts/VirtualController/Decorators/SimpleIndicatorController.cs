using UnityEngine;

public class SimpleIndicatorController : MonoBehaviour
{
    [SerializeField] private VirtualIndicator m_indicator;
    [SerializeField] private VirtualInput<bool> m_controllingInput;

    [SerializeField] private bool m_inverse = false;

    private void Start()
    {
        m_controllingInput.OnValueChanged += OnInputValueChanged;
        OnInputValueChanged(m_controllingInput.Value);
    }

    private void OnInputValueChanged(bool value)
    {
        m_indicator.SetEnable(value ^ m_inverse);
    }
}
