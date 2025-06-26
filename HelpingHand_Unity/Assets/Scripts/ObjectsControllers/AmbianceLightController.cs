using System;

using UnityEngine;

using UnityUtility.Timer;

public class AmbianceLightController : MonoBehaviour
{
    [SerializeField] private ContinuousLightController[] m_lightControllers;

    [SerializeField] private float m_transitionTime;

    [NonSerialized] private Timer m_transitionTimer;
    [NonSerialized] private bool m_transitionIn = false;

    public void Initialize()
    {
        m_transitionTimer = new Timer(m_transitionTime, false);
    }

    public void SetFocus(bool focus)
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

        m_transitionIn = focus;

        if (focus)
        {
            foreach (ContinuousLightController lightController in m_lightControllers)
            {
                lightController.SettingsContainer.Spot.gameObject.SetActive(true);
                lightController.SettingsContainer.ExternalIntensityMultiplier = 0.0f;
            }
        }
    }

    private void Update()
    {
        if (!m_transitionTimer.IsRunning)
        {
            return;
        }

        bool finished = m_transitionTimer.Update(Time.deltaTime);
        float progress = finished ? 1.0f : m_transitionTimer.Progress;
        if (finished)
        {
            m_transitionTimer.Stop();
        }

        foreach (ContinuousLightController lightController in m_lightControllers)
        {
            if (!m_transitionIn && finished)
            {
                lightController.SettingsContainer.Spot.gameObject.SetActive(false);
            }
            lightController.SettingsContainer.ExternalIntensityMultiplier = m_transitionIn ? progress : 1.0f - progress;
        }
    }
}
