using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.ObservableFields;

[Serializable]
public class GameOptionsManager
{
    [Title("Options", bold: false)]
    public ObservableField<bool> IsHighContrast = new ObservableField<bool>(false);
    public ObservableField<float> GameSpeed = new ObservableField<float>(1.0f);
    public ObservableField<bool> IsWindowed = new ObservableField<bool>(false);
    public ObservableField<DialogueReadMode> DialogueReadMode = new ObservableField<DialogueReadMode>(global::DialogueReadMode.Auto);
    public ObservableField<SubtitleSize> SubtitleSize = new ObservableField<SubtitleSize>(global::SubtitleSize.Medium);
    public ObservableField<Color> SubtitleColor = new ObservableField<Color>(Color.black);
    public ObservableField<float> SubtitleOpacity = new ObservableField<float>(1.0f);

    [Title("Font sizes", bold: false)]
    [SerializeField] private float m_smallFontSize = 80.0f;
    [SerializeField] private float m_mediumFontSize = 100.0f;
    [SerializeField] private float m_largeFontSize = 120.0f;


    public float ToFontSize(SubtitleSize size)
    {
        return size switch
        {
            global::SubtitleSize.Small => m_smallFontSize,
            global::SubtitleSize.Medium => m_mediumFontSize,
            global::SubtitleSize.Large => m_largeFontSize,
            _ => m_mediumFontSize,
        };
    }
}
