using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using UnityUtility.Extensions;

public class SceneLightsController : MonoBehaviour
{
    [Serializable]
    private class LightAmbiance
    {
        [SerializeField] private AmbianceLightController m_controller;
        [SerializeField] private EntityState m_entityState;

        [NonSerialized] public bool IsActive;
        [NonSerialized] private List<Light> m_allLights;

        private EntityState m_globalLightsToggle;

        public void Init(EntityState globalLightsToggle)
        {
            m_controller.Initialize();
            IsActive = false;
            m_globalLightsToggle = globalLightsToggle;
            m_entityState.AddListener(OnStateChanged);
            m_globalLightsToggle.AddListener(OnStateChanged);
            OnStateChanged(false);
        }

        public void Dispose()
        {
            m_entityState.RemoveListener(OnStateChanged);
            m_globalLightsToggle.RemoveListener(OnStateChanged);
        }

        private void OnStateChanged(bool state)
        {
            IsActive = m_entityState.Value && m_globalLightsToggle.Value;
            m_controller.SetFocus(IsActive);
        }
    }

    [SerializeField] private EntityState m_sceneLightsActive;
    [SerializeField] private EntityState m_noAmbianceSelected;
    [SerializeField] private FloatVariable m_sceneSpotsIntensity;
    [SerializeField] [Range(0, 1)] private float m_minIntensityPercentage = 0.10f;
    [SerializeField] private LightAmbiance[] m_ambiances;
    
    [NonSerialized] private Dictionary<Light, (float minIntensity, float maxIntensity)> m_lightSettings;

    private void Awake()
    {
        Light[] m_allLights = GetComponentsInChildren<Light>();
        m_lightSettings = new();
        foreach (Light light in m_allLights)
        {
            // if (light.TryGetComponent(out ContinuousLightController continuous))
            // {
            //     m_lightSettings[light] = (Mathf.Min(continuous.SettingsContainer.MinSettings.Intensity, continuous.SettingsContainer.MaxSettings.Intensity) * m_minIntensityPercentage,
            // }
            // else
            if (light.TryGetComponent(out DiscreetLightController discreet))
            {
                m_lightSettings[light] = (discreet.SettingsContainer.MinSettings.Intensity, discreet.SettingsContainer.MaxSettings.Intensity);
            }
            else
            {
                m_lightSettings[light] = (light.intensity * m_minIntensityPercentage, light.intensity);
            }
        }
    }

    private void Start()
    {
        m_ambiances.ForEach(a => a.Init(m_sceneLightsActive));
    }

    private void OnEnable()
    {
        m_sceneSpotsIntensity.AddListener(OnIntensityChanged);
    }

    private void OnDisable()
    {           
        m_sceneSpotsIntensity.RemoveListener(OnIntensityChanged);
    }


    private void OnIntensityChanged(float value)
    {
        foreach (Light light in m_lightSettings.Keys)
        {
            light.intensity = Mathf.Lerp(m_lightSettings[light].minIntensity, m_lightSettings[light].maxIntensity, value);
        }
    }
    
    private void Update()
    {
        m_noAmbianceSelected.Value = m_sceneLightsActive.Value && m_ambiances.All(a => !a.IsActive);
    }

    private void OnDestroy()
    {
        m_ambiances.ForEach(a => a.Dispose());
    }
}
