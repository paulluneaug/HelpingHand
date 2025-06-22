using Sirenix.OdinInspector;

using UnityEngine;

[RequireComponent(typeof(VirtualIndicator))]
public class SimpleIndicatorController : MonoBehaviour
{
    [SerializeField][Required] private VirtualIndicator m_indicator;
    [SerializeField][Required] private VirtualInput<bool> m_controllingInput;

    [SerializeField] private bool m_inverse = false;

    private void OnEnable()
    {
        m_controllingInput.OnValueChanged -= OnInputValueChanged;
        m_controllingInput.OnValueChanged += OnInputValueChanged;

        m_controllingInput.OnActivate -= OnInputActivate;
        m_controllingInput.OnActivate += OnInputActivate;

        m_controllingInput.OnDeactivate -= OnInputDeactivate;
        m_controllingInput.OnDeactivate += OnInputDeactivate;
    }

    private void OnDisable()
    {
        m_controllingInput.OnValueChanged -= OnInputValueChanged;
        m_controllingInput.OnActivate -= OnInputActivate;

        m_controllingInput.OnDeactivate -= OnInputDeactivate;
    }

    private void Awake()
    {
        m_indicator = GetComponent<VirtualIndicator>();
    }

    private void Start()
    {
        OnInputValueChanged(m_controllingInput.Value);
    }

    private void OnInputValueChanged(bool value)
    {
        if (!m_controllingInput.IsActive)
        {
            return;
        }
        m_indicator.SetEnable(value ^ m_inverse);
    }

    private void OnInputActivate()
    {
        OnInputValueChanged(m_controllingInput.Value);
    }

    private void OnInputDeactivate()
    {
        m_indicator.SetEnable(false);
    }
}
