using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

using TitleAlignments = Sirenix.OdinInspector.TitleAlignments;

public class SoundbankManager : MonoBehaviour
{
    #region Soundbanks list
    [TitleGroup("Startup SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [InfoBox("➔<b>LoadStartupSoundbanks()</b>", InfoMessageType.None)]
    [SerializeField] private List<AK.Wwise.Bank> m_soundbanks;
    #endregion

    [TitleGroup("First Level SoundBanks", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [InfoBox("➔<b>LoadFirstLevelSoundbanks()</b>", InfoMessageType.None)]
    [SerializeField] private List<AK.Wwise.Bank> m_firstLevelSoundbanks;
    public void LoadStartupSoundbanks()
    {
        if (m_soundbanks.Count > 0) // Dans le cas où l'on a des soundbanks
        {
            foreach (AK.Wwise.Bank bank in m_soundbanks) //Load toutes les soundbanks dans la liste
            {
                bank.Load();
            }
        }
        else
        {
            Debug.LogError("No SoundBanks found in the list. Please add soundbanks to the Audiomanager :)");
        }
    }
    public void LoadFirstLevelSoundbanks()
    {
        if (m_soundbanks.Count > 0) // Dans le cas où l'on a des soundbanks
        {
            foreach (AK.Wwise.Bank bank in m_firstLevelSoundbanks) //Load toutes les soundbanks dans la liste
            {
                bank.Load();
            }
        }
        else
        {
            Debug.LogError("No SoundBanks found in the list. Please add soundbanks to the Audiomanager :)");
        }
    }

}
