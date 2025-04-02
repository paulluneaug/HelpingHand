using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

using UnityUtility.Singletons;

namespace MusicManager
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
        GameplayThirdSection,
        PauseMenu,
        LevelWin,
        LevelLose
    }

    public class MusicManager : MonoBehaviourSingleton<MusicManager>
    {
        public static MusicManager Instance; //Pour accéder à ce script de partout facilement : une seule instance


        #region GameState variables
        [Header("Game States")]
        [ReadOnly][SerializeField] private AudioGameState currentGameState;

        [Header("Music States")]
        [SerializeField] private AK.Wwise.State Music_Gameplay1stSection;
        [SerializeField] private AK.Wwise.State Music_Gameplay2ndSection;
        [SerializeField] private AK.Wwise.State Music_Gameplay3rdSection;
        [SerializeField] private AK.Wwise.State Music_MainMenu;
        [SerializeField] private AK.Wwise.State Music_Level_Lose;
        [SerializeField] private AK.Wwise.State Music_Level_Win; 
        [SerializeField] private AK.Wwise.State Music_None;

        [Header("RTPC")]
        [SerializeField][Range(0f, 1f)] private float Music_FirstLayer = 0;
        [SerializeField][Range(0f, 1f)] private float Music_SecondLayer = 0;


        [ReadOnly][SerializeField] private AudioMusicState currentMusicState;
        #endregion

        #region Sound Events
        [Header("Wwise Music Events")]
        [SerializeField] public AK.Wwise.Event MainMusic_Play;
        [SerializeField] public AK.Wwise.Event MainMusic_Stop;
        
        #endregion

        public override void Initialize()
        {
            base.Initialize();
            SetAudioMusicState(AudioMusicState.None); //On initialise l'état de la musique à None (reset)  
        }

        void Start()
        {
            SetAudioMusicState(AudioMusicState.MainMenu); //On initialise l'état de la musique à MainMenu

            MainMusic_Play.Post(gameObject); //On joue la musique principale

            AkSoundEngine.SetRTPCValue("RTPC_Music_FirstLayer", Music_FirstLayer);
            AkSoundEngine.SetRTPCValue("RTPC_Music_SecondLayer", Music_SecondLayer);
        }

        private void Update()
        {
            AkSoundEngine.SetRTPCValue("RTPC_Music_FirstLayer", Music_FirstLayer);
            AkSoundEngine.SetRTPCValue("RTPC_Music_SecondLayer", Music_SecondLayer);
        }

        public void SetAudioMusicState(AudioMusicState MusicState) // Change l'état des states liés à la musique
        {
            if (MusicState == currentMusicState)
            {
                Debug.Log("MusicState is already" + MusicState + "."); //On ne change pas l'état si c'est déjà le bon
                return;
            }

            switch(MusicState)
            {
                default: //Cas pas défaut = mainmenu
                case AudioMusicState.MainMenu:
                    Music_MainMenu.SetValue();
                    break;
                case AudioMusicState.GameplayFirstSection:
                    Music_Gameplay1stSection.SetValue();
                    break;
                case AudioMusicState.LevelLose:
                    Music_Level_Lose.SetValue();
                    break;
                case AudioMusicState.None:   
                    Music_None.SetValue();
                    break;
                
            }
            currentMusicState = MusicState; //On met à jour l'état actuel
            Debug.Log("New Wwise GameState: " + MusicState + ".");
        }

    }



}
