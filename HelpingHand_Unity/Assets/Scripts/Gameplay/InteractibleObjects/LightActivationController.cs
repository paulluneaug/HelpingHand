using Sirenix.OdinInspector;

using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightActivationController : SerializedMonoBehaviour
{
    [SerializeField]
    private InputTrigger m_litTrigger;

    [SerializeField]
    private InputTrigger m_outTrigger;

    [SerializeField]
    private EntityState m_litState;

    [SerializeField]
    private EntityState m_unlitState;

    private Light m_light;

    private void Awake()
    {
        m_light = GetComponent<Light>();
        m_litState.SetValueWithoutNotify(m_light.enabled);
        m_unlitState.SetValueWithoutNotify(!m_light.enabled);
    }

    private void Start()
    {
        m_litTrigger.Initialize();
        m_outTrigger.Initialize();
        m_litTrigger.RaiseTriggerEvent -= OnLitTriggerRaised;
        m_litTrigger.RaiseTriggerEvent += OnLitTriggerRaised;
        m_outTrigger.RaiseTriggerEvent -= OnOutTriggerRaised;
        m_outTrigger.RaiseTriggerEvent += OnOutTriggerRaised;
        m_litState?.SetValueWithoutNotify(m_light.enabled);
        m_unlitState?.SetValueWithoutNotify(!m_light.enabled);
    }

    private void OnLitTriggerRaised()
    {
        m_light.enabled = true;
        m_litState.Value = true;
        m_unlitState.Value = false;
    }

    private void OnOutTriggerRaised()
    {
        m_light.enabled = false;
        m_litState.Value = false;
        m_unlitState.Value = true;
    }
}
