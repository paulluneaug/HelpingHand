using UnityEngine;

public class FloatOptionController : BaseOptionController<float>
{
    public override float ReadFromPlayerPrefs(float defaultValue)
    {
        return PlayerPrefs.GetFloat(m_playerPrefName, defaultValue);
    }

    public override void WriteToPlayerPrefs(float value)
    {
        PlayerPrefs.SetFloat(m_playerPrefName, value);
    }
}
