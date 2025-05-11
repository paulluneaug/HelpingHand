using Sirenix.OdinInspector;

using UnityEngine;

public class AmbientLightController : SingleSliderInteractiveObject
{
    [Range(0, 1)] [SerializeField]
    private float m_startValue;

    [SerializeField] [MinMaxSlider(0, 1, true)]
    private Vector2 m_minMaxIntensity;

    private Vector3 m_hsv;
    
    protected override void Start()
    {
        base.Start();
        m_masterSlider.SetValueWithoutNotify(m_startValue);
        m_masterSlider.OnSliderValueChanged += OnSliderValueChanged;
        OnSliderValueChanged(m_startValue);
        Color.RGBToHSV(RenderSettings.ambientLight, out m_hsv.x, out m_hsv.y, out m_hsv.z);
    }

    private new void OnSliderValueChanged(float value)
    {
        m_hsv.z = Mathf.Lerp(m_minMaxIntensity.x, m_minMaxIntensity.y, value);
        RenderSettings.ambientLight = Color.HSVToRGB(m_hsv.x, m_hsv.y, m_hsv.z);
    }
}
