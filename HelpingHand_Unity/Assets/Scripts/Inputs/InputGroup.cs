using Events;

using Sirenix.OdinInspector;

using UnityEngine;

public class InputGroup : MonoBehaviour, ILateStarter
{
    [SerializeField]
    private BaseGameEvent[] m_inputs;

    [SerializeField]
    private IndicatorState m_indicatorState;

    [SerializeField]
    private bool m_activateAtStart = false;

    private readonly bool m_isActive;

    public void LateStart()
    {
        if (m_indicatorState != null)
        {
            m_indicatorState.Value = m_activateAtStart;
        }

        foreach (BaseGameEvent input in m_inputs)
        {
            input.IsActive = m_activateAtStart;
        }
    }

    public void SetActive(bool isActive)
    {
        m_indicatorState.Value = isActive;
        foreach (BaseGameEvent input in m_inputs)
        {
            input.IsActive = isActive;
        }
    }

    [Button("Enable")] [HorizontalGroup("Buttons")]
    public void Enable()
    {
        SetActive(true);
    }

    [Button("Disable")] [HorizontalGroup("Buttons")]
    public void Disable()
    {
        SetActive(false);
    }
}
