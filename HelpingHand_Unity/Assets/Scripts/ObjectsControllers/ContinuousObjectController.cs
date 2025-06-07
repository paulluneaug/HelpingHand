using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Easings;
using UnityUtility.MathU;

public abstract class ContinuousObjectController<TSettingsContainer> : MonoBehaviour, IBaseVariableContainer<float>
    where TSettingsContainer : IObjectSettingsContainer
{
    public BaseVariable<float> Variable => m_controllingVariable;

    [Title("Variable")]
    [SerializeField] private BaseVariable<float> m_controllingVariable;

    [Title("Settings")]
    [SerializeField] private TSettingsContainer m_settings;

    [Title("Transition")]
    [SerializeField] private float m_smoothHalfLife = 0.1f;
    [SerializeField] private Easings.EasingFunction m_easingFunction;

    [Title("Debug")]
    [SerializeField, Range(0.0f, 1.0f)] private float m_debugProgress;

    [NonSerialized] private float m_currentValue;
    [NonSerialized] private float m_target;


    private void Start()
    {
        m_controllingVariable.AddListener(OnVariableChanged);
        m_controllingVariable.OnActivate += OnVariableActivate;

        m_currentValue = m_controllingVariable.Value;
        OnVariableChanged(m_controllingVariable.Value);
    }

    private void Update()
    {
        m_currentValue = Easings.Ease(MathUf.SmoothLerp(m_currentValue, m_target, Time.deltaTime, m_smoothHalfLife), m_easingFunction);
        m_settings.UpdateSettings(m_currentValue);
    }

    private void OnDestroy()
    {
        m_controllingVariable.RemoveListener(OnVariableChanged);
        m_controllingVariable.OnActivate -= OnVariableActivate;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying || m_settings == null)
        {
            return;
        }
        m_settings.UpdateSettings(Easings.Ease(m_debugProgress, m_easingFunction));
    }
#endif

    private void OnVariableActivate()
    {
        OnVariableChanged(m_controllingVariable.Value);
    }

    private void OnVariableChanged(float variableValue)
    {
        m_target = variableValue;
    }
}

