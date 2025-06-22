using System;

using UnityEngine;

public abstract class EnumOptionController<TEnum> : BaseOptionController<TEnum>
    where TEnum : struct
{
    public override TEnum ReadFromPlayerPrefs(TEnum defaultValue)
    {
        return Enum.Parse<TEnum>(PlayerPrefs.GetString(m_playerPrefName, defaultValue.ToString()));
    }

    public override void WriteToPlayerPrefs(TEnum value)
    {
        PlayerPrefs.SetString(m_playerPrefName, value.ToString());
    }
}
