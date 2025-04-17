using System.Collections.Generic;

using UnityEngine;

using UnityUtility.Singletons;

public class SoundBankLoader : Singleton<SoundBankLoader>
{
    #region Soundbanks list
    [Header("Startup SoundBanks")] //Liste de nos soundbanks
    [SerializeField] private List<AK.Wwise.Bank> Soundbanks;
    #endregion

    private void Awake()
    {
        Initialize(); //Appel de la fonction d'initialisation
    }

    private void Initialize()
    {
        LoadSoundbanks(); //Appel de la fonction de chargement des soundbanks
    }

    private void LoadSoundbanks() //Load les soundbanks (pas encore dynamiquement)
    {
        if (Soundbanks.Count > 0) // Dans le cas où l'on a des soundbanks
        {
            foreach (AK.Wwise.Bank bank in Soundbanks) //Load toutes les soundbanks dans la liste
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
