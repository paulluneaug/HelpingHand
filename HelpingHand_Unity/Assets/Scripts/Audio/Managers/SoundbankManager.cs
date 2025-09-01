using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

using TitleAlignments = Sirenix.OdinInspector.TitleAlignments;

public class SoundbankManager : MonoBehaviour
{
    #region Startup
    [TitleGroup("Startup SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [InfoBox("➔ <b>LoadStartupSoundbanks()</b> / <b>UnloadStartupSoundbanks()</b>", InfoMessageType.None)]
    [SerializeField] private List<AK.Wwise.Bank> m_soundbanks;
    #endregion

    #region Gameplay
    [TitleGroup("Gameplay SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [InfoBox("➔ <b>LoadGameplaySoundbanks()</b> / <b>UnloadGameplaySoundbanks()</b>", InfoMessageType.None)]
    [SerializeField] private List<AK.Wwise.Bank> m_gameplaySoundbanks;
    #endregion

    #region Onboarding
    [TitleGroup("Onboarding SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [InfoBox("➔ <b>LoadOnboardingSoundbanks()</b> / <b>UnloadOnboardingSoundbanks()</b>", InfoMessageType.None)]
    [SerializeField] private List<AK.Wwise.Bank> m_onboardingSoundbanks;
    #endregion

    #region Roue
    [TitleGroup("Roue SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [InfoBox("➔ <b>LoadRoueSoundbanks()</b> / <b>UnloadRoueSoundbanks()</b>", InfoMessageType.None)]
    [SerializeField] private List<AK.Wwise.Bank> m_roueSoundbanks;
    #endregion

    #region Équipement
    [TitleGroup("Équipement SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [InfoBox("➔ <b>LoadEquipementSoundbanks()</b> / <b>UnloadEquipementSoundbanks()</b>", InfoMessageType.None)]
    [SerializeField] private List<AK.Wwise.Bank> m_equipementSoundbanks;
    #endregion

    #region Combat
    [TitleGroup("Combat SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [InfoBox("➔ <b>LoadCombatSoundbanks()</b> / <b>UnloadCombatSoundbanks()</b>", InfoMessageType.None)]
    [SerializeField] private List<AK.Wwise.Bank> m_combatSoundbanks;
    #endregion

    #region Fin
    [TitleGroup("Fin SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [InfoBox("➔ <b>LoadFinSoundbanks()</b> / <b>UnloadFinSoundbanks()</b>", InfoMessageType.None)]
    [SerializeField] private List<AK.Wwise.Bank> m_finSoundbanks;
    #endregion

    // --- LOAD ---
    public void LoadStartupSoundbanks()
    {
        LoadBanks(m_soundbanks, "Startup");
    }

    public void LoadGameplaySoundbanks()
    {
        LoadBanks(m_gameplaySoundbanks, "Gameplay");
    }

    public void LoadOnboardingSoundbanks()
    {
        LoadBanks(m_onboardingSoundbanks, "Onboarding");
    }

    public void LoadRoueSoundbanks()
    {
        LoadBanks(m_roueSoundbanks, "Roue");
    }

    public void LoadEquipementSoundbanks()
    {
        LoadBanks(m_equipementSoundbanks, "Équipement");
    }

    public void LoadCombatSoundbanks()
    {
        LoadBanks(m_combatSoundbanks, "Combat");
    }

    public void LoadFinSoundbanks()
    {
        LoadBanks(m_finSoundbanks, "Fin");
    }

    // --- UNLOAD ---
    public void UnloadStartupSoundbanks()
    {
        UnloadBanks(m_soundbanks, "Startup");
    }

    public void UnloadGameplaySoundbanks()
    {
        UnloadBanks(m_gameplaySoundbanks, "Gameplay");
    }

    public void UnloadOnboardingSoundbanks()
    {
        UnloadBanks(m_onboardingSoundbanks, "Onboarding");
    }

    public void UnloadRoueSoundbanks()
    {
        UnloadBanks(m_roueSoundbanks, "Roue");
    }

    public void UnloadEquipementSoundbanks()
    {
        UnloadBanks(m_equipementSoundbanks, "Équipement");
    }

    public void UnloadCombatSoundbanks()
    {
        UnloadBanks(m_combatSoundbanks, "Combat");
    }

    public void UnloadFinSoundbanks()
    {
        UnloadBanks(m_finSoundbanks, "Fin");
    }

    // --- GLOBAL LOAD/UNLOAD ---
    public void LoadAllSoundbanks()
    {
        LoadStartupSoundbanks();
        LoadGameplaySoundbanks();
        LoadOnboardingSoundbanks();
        LoadRoueSoundbanks();
        LoadEquipementSoundbanks();
        LoadCombatSoundbanks();
        LoadFinSoundbanks();
    }

    public void UnloadAllSoundbanks()
    {
        UnloadStartupSoundbanks();
        UnloadGameplaySoundbanks();
        UnloadOnboardingSoundbanks();
        UnloadRoueSoundbanks();
        UnloadEquipementSoundbanks();
        UnloadCombatSoundbanks();
        UnloadFinSoundbanks();
    }




    // --- Shared Methods ---
    private void LoadBanks(List<AK.Wwise.Bank> banks, string context)
    {
        if (banks != null && banks.Count > 0)
        {
            foreach (var bank in banks)
            {
                bank.Load();
            }
        }
        else
        {
            Debug.LogError($"[Wwise] No SoundBanks found for '{context}'. Please check AudioManager references.");
        }
    }

    private void UnloadBanks(List<AK.Wwise.Bank> banks, string context)
    {
        if (banks != null && banks.Count > 0)
        {
            foreach (var bank in banks)
            {
                bank.Unload();
            }
        }
        else
        {
            Debug.LogWarning($"[Wwise] Nothing to unload for '{context}'.");
        }
    }
}
