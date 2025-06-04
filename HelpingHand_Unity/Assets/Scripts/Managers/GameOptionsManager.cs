using System;

using UnityEngine;

using UnityUtility.ObservableFields;

[Serializable]
public class GameOptionsManager
{
    public ObservableField<bool> IsHighContrast = new ObservableField<bool>(false);
    public ObservableField<float> GameSpeed = new ObservableField<float>(1.0f);
    public ObservableField<bool> IsWindowed = new ObservableField<bool>(false);
    public ObservableField<DialogueReadMode> DialogueReadMode = new ObservableField<DialogueReadMode>(global::DialogueReadMode.Auto);
    public ObservableField<SubtitleSize> SubtitleSize = new ObservableField<SubtitleSize>(global::SubtitleSize.Medium);
    public ObservableField<Color> SubtitleColor = new ObservableField<Color>(UnityEngine.Color.black);
    public ObservableField<float> SubtitleOpacity = new ObservableField<float>(1.0f);
}
