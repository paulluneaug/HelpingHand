using System.Collections.Generic;
using UnityEngine;
using System;

namespace SoundSystemTanguy
{

    public enum AudioGameState
    {
        None,
        MainMenu,
        Gameplay,
        Paused,
        GameOver
    }

    public enum AudioMusicState
    {
        None,
        MainMenu,
        GameplayFirstSection,
        GameplaySecondSection,
        PauseMenu,
        LevelWin,
        LevelLose
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance; //Pour accéder à ce script de partout facilement : une seule instance
        private bool bIsInitialized = false;

        [Header("Startup SoundBanks")] //Liste de nos soundbanks
        [SerializeField] private List<AK.Wwise.Bank> Soundbanks;

        #region GameState variables
        [Header("Game State")]
        [SerializeField] private AK.Wwise.State Game_Gameplay;
        [SerializeField] private AK.Wwise.State Game_MainMenu;
        [SerializeField] private AK.Wwise.State Game_Paused;
        [SerializeField] private AK.Wwise.State Game_GameOver; 

        [Header("Music States")]
        [SerializeField] private AK.Wwise.State Music_PauseMenu;
        [SerializeField] private AK.Wwise.State Music_MainMenu;
        [SerializeField] private AK.Wwise.State Music_Level_Start;
        [SerializeField] private AK.Wwise.State Music_Level_Lose;
        [SerializeField] private AK.Wwise.State Music_Level_Win;

        internal static void SetAudioGameState(AudioGameState mainMenu)
        {
            throw new NotImplementedException();
        }

        internal void SetAudioMusicState(AudioMusicState levelLose)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void Awake()
        {
            Initialize(); //Appel de la fonction d'initialisation
        }

        void Initialize()
        {
            //Singleton
            if (Instance==null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Debug.LogWarning("De multiples instances de AudioManager ont été trouvées. Veuillez vous assurer qu'il n'y a qu'une seule instance dans la scène.");
                Destroy(this);
            }

            if(!bIsInitialized)
            {
                LoadSoundbanks(); //Appel de la fonction de chargement des soundbanks
            }
        }
        void LoadSoundbanks()
        {
                if(Soundbanks.Count > 0) // Dans le cas où l'on a des soundbanks
                {
                    foreach(AK.Wwise.Bank bank in Soundbanks) //Load toutes les soundbanks dans la liste
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

}
