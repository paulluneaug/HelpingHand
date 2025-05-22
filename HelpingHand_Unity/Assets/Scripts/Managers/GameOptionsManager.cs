using System;

using UnityUtility.ObservableFields;

[Serializable]
public class GameOptionsManager
{
    public bool IsInvincible = false;
    public ObservableField<bool> IsHighContrast = new ObservableField<bool>(false);
    public ObservableField<float> GameSpeed = new ObservableField<float>(1.0f);
    public ObservableField<int> Sensitivity = new ObservableField<int>(50);
    public ObservableField<bool> IsWindowed = new ObservableField<bool>(false);
}
