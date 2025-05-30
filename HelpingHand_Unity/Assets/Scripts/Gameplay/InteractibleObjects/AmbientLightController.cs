using System;

using Sirenix.OdinInspector;

using UnityEngine;

public class AmbientLightController : MonoBehaviour
{
    [SerializeField]
    private FloatVariable m_inputEvent;

    [Range(0, 1)]
    [SerializeField]
    private float m_startValue;

    [SerializeField]
    [MinMaxSlider(0, 1, true)]
    private Vector2 m_minMaxIntensity;

    [SerializeField]
    private float m_smoothTime = .2f;

    private Vector3 m_hsv;

    private float m_targetIntensity;
    private float m_currentVelocity;

    private void OnEnable()
    {
        m_inputEvent.OnActivate -= OnInputEventActivate;
        m_inputEvent.OnActivate += OnInputEventActivate;
        m_inputEvent.RemoveListener(OnValueChanged);
        m_inputEvent.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        m_inputEvent.OnActivate -= OnInputEventActivate;
        m_inputEvent.RemoveListener(OnValueChanged);
    }

    private void OnInputEventActivate()
    {
        OnValueChanged(m_inputEvent.Value);
    }

    protected void Start()
    {
        OnValueChanged(m_inputEvent.Value);
        Color.RGBToHSV(RenderSettings.ambientLight, out m_hsv.x, out m_hsv.y, out m_hsv.z);
    }

    private void OnValueChanged(float value)
    {
        m_targetIntensity = Mathf.Lerp(m_minMaxIntensity.x, m_minMaxIntensity.y, value);
    }

    private void Update()
    {
        if (!Mathf.Approximately(m_hsv.z, m_targetIntensity))
        {
            m_hsv.z = Mathf.SmoothDamp(m_hsv.z, m_targetIntensity, ref m_currentVelocity, m_smoothTime);
            RenderSettings.ambientLight = Color.HSVToRGB(m_hsv.x, m_hsv.y, m_hsv.z);
        }
    }
}
