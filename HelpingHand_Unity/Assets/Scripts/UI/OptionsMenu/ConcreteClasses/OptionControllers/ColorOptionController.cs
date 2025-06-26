using System;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;
using UnityUtility.MathU;


[RequireComponent(typeof(RectTransform))]
public class ColorOptionController : MonoBehaviour
{
    public Color Value => m_value;

    public event Action<Color> OnValueChanged;

    [Title("Sliders")]
    [SerializeField] private FloatOptionController m_hueController;
    [Space]
    [SerializeField] private FloatOptionController m_svController;
    [SerializeField] private Image m_svSliderBackground;

    [Title("Preview")]
    [SerializeField] private Image m_previewImage;

    [Title("Materials")]
    [SerializeField] private Material m_svGradientMaterial;

    [NonSerialized] private float m_hue;
    [NonSerialized] private float m_sv;
    [NonSerialized] private Color m_value;


    private void Awake()
    {
        m_svSliderBackground.material = Instantiate(m_svGradientMaterial);

        m_hueController.OnValueChanged += OnHueChanged;
        m_svController.OnValueChanged += OnSVChanged;

        OnHueChanged(m_hueController.Value);
        OnSVChanged(m_svController.Value);
    }

    private void OnDestroy()
    {
        m_hueController.OnValueChanged -= OnHueChanged;
        m_svController.OnValueChanged -= OnSVChanged;
    }

    private void OnSVChanged(float sv)
    {
        m_sv = sv;
        OnColorChanged();
    }

    private void OnHueChanged(float hue)
    {
        m_hue = hue;

        m_svSliderBackground.materialForRendering.SetFloat("_Hue", m_hue);
        OnColorChanged();
    }

    private void OnColorChanged()
    {
        (float s, float v) = ComputeSV(m_sv);
        m_value = Color.HSVToRGB(m_hue, s, v);

        OnValueChanged?.Invoke(m_value);

        if (m_previewImage != null)
        {
            m_previewImage.color = m_value;
        }
    }

    private (float, float) ComputeSV(float sv)
    {
        float s = 1 - MathUf.Clamp01(sv.RemapTo01(0.5f, 1.0f));
        float v = MathUf.Clamp01(sv.RemapTo01(0.0f, 0.5f));
        return (s, v);
    }

    private float ComputeFactor(float s, float v)
    {
        if (s == 0)
        {
            return 1.0f;
        }

        if (v == 0)
        {
            return 0.0f;
        }

        if (v > s)
        {
            return 0.5f + (1.0f - s / v) / 2.0f;
        }
        return (v / s) / 2.0f;
    }
}
