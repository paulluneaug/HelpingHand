using Sirenix.OdinInspector;

using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightActivationController : SerializedMonoBehaviour
{
    [SerializeField]
    private InputTrigger m_trigger;

    [SerializeField]
    private bool m_doActivateLight = true;

    private Light m_light;

    private void Awake()
    {
        m_light = GetComponent<Light>();
    }

    private void Start()
    {
        m_trigger.Initialize();
        m_trigger.RaiseTriggerEvent -= OnTriggerRaised;
        m_trigger.RaiseTriggerEvent += OnTriggerRaised;
    }

    private void OnTriggerRaised()
    {
        m_light.enabled = m_doActivateLight;
    }
}
