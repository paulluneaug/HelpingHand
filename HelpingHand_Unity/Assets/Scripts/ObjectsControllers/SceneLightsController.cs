using System;
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

    private void Start()
    {
        m_ambiances.ForEach(a => a.Init(m_sceneLightsActive));
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
