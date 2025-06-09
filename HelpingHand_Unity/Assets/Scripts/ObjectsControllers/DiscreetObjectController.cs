using Sirenix.OdinInspector;
using System;

using UnityEngine;

using UnityUtility.Easings;
using UnityUtility.Timer;

public class DiscreetObjectController<TSettingsContainer> : MonoBehaviour, IBaseVariableContainer<bool>
    where TSettingsContainer : IObjectSettingsContainer
{
    public BaseVariable<bool> Variable => m_controllingVariable;

    [Title("Variable")]
    [SerializeField] private BaseVariable<bool> m_controllingVariable;

    [Title("Settings")]
    [SerializeField] private TSettingsContainer m_settings;

    [Title("Transition")]
    [SerializeField] private float m_transitionTime;
    [SerializeField] private Easings.EasingFunction m_easingFunction = Easings.EasingFunction.Linear;

    [Title("Debug")]
    [SerializeField, Range(0.0f, 1.0f)] private float m_debugProgress;

    [NonSerialized] private Timer m_transitionTimer;

    private void Start()
    {
        m_settings.Init();

        m_transitionTimer = new Timer(m_transitionTime, false);
        m_controllingVariable.AddListener(OnVariableChanged);
        m_controllingVariable.OnActivate += OnVariableActivate;
    }

    private void Update()
    {
        if (!m_transitionTimer.IsRunning)
        {
            return;
        }

        bool finished = m_transitionTimer.Update(Time.deltaTime);
        if (finished)
        {
            m_transitionTimer.Stop();
        }
        float progress = finished ? 1.0f : m_transitionTimer.Progress;

        if (!m_controllingVariable.Value)
        {
            progress = 1.0f - progress;
        }

        m_settings.UpdateSettings(Easings.Ease(progress, m_easingFunction));
    }

    private void OnDestroy()
    {
        m_settings.Dispose();

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

    private void OnVariableChanged(bool variableValue)
    {
        if (m_transitionTimer.IsRunning)
        {
            float timeToTarget = (1.0f - m_transitionTimer.Progress) * m_transitionTimer.Duration;
            m_transitionTimer.Reset();
            _ = m_transitionTimer.Update(timeToTarget);
        }
        else
        {
            m_transitionTimer.Reset();
        }

        m_transitionTimer.Start();
    }
}
