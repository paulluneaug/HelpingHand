using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.ObservableFields;

[Serializable]
public class GameOptionsManager
{
    [Title("Options", bold: false)]
    public ObservableField<bool> IsWindowed = new ObservableField<bool>(false);
    public ObservableField<DialogueReadMode> DialogueReadMode = new ObservableField<DialogueReadMode>(global::DialogueReadMode.Auto);
    public ObservableField<SubtitleSize> SubtitleSize = new ObservableField<SubtitleSize>(global::SubtitleSize.Medium);
    public ObservableField<Color> SubtitleColor = new ObservableField<Color>(Color.black);
    public ObservableField<float> SubtitleBackgroundOpacity = new ObservableField<float>(1.0f);
    public ObservableField<Color> SubtitleBackgroundColor = new ObservableField<Color>(Color.black);

    [Title("Font sizes", bold: false)]
    [SerializeField] private float m_smallFontSize = 60.0f;
    [SerializeField] private float m_mediumFontSize = 70.0f;
    [SerializeField] private float m_largeFontSize = 80.0f;

    [Title("Background sizes", bold: false)]
    [SerializeField] private float m_smallBackgroundWidth = 1850.0f;
    [SerializeField] private float m_mediumBackgroundWidth = 2150.0f;
    [SerializeField] private float m_largeBackgroundWidth = 2400.0f;

    [Title("Resolution")]
    [SerializeField] private int m_windowedWidth = 1920;
    [SerializeField] private int m_windowedHeight = 1080;

    private Resolution m_screenNativeResolution;

    public void Initialize()
    {
        m_screenNativeResolution = Screen.resolutions[0];
        IsWindowed.OnValueChanged += OnWindowedModeChanged;
    }

    public void Dispose()
    {
        IsWindowed.OnValueChanged -= OnWindowedModeChanged;
    }

    public float ToFontSize(SubtitleSize size)
    {
        return size switch
        {
            global::SubtitleSize.Small => m_smallFontSize,
            global::SubtitleSize.Medium => m_mediumFontSize,
            global::SubtitleSize.Large => m_largeBackgroundWidth,
            _ => m_mediumFontSize,
        };
    }

    public float ToBackgroundWidth(SubtitleSize size)
    {
        return size switch
        {
            global::SubtitleSize.Small => m_smallBackgroundWidth,
            global::SubtitleSize.Medium => m_mediumBackgroundWidth,
            global::SubtitleSize.Large => m_largeFontSize,
            _ => m_mediumFontSize,
        };
    }

    private void OnWindowedModeChanged(bool isWindowed)
    {
        if (!isWindowed)
        {
            Screen.SetResolution(m_screenNativeResolution.width, m_screenNativeResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(m_windowedWidth, m_windowedHeight, FullScreenMode.Windowed);
        }
    }
}
