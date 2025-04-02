using System.Collections.Generic;
using UnityEngine;

namespace WwiseAudioManager
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

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance; //Pour accéder à ce script de partout facilement : une seule instance
        private bool bIsInitialized = false;

        #region Soundbanks list
        [Header("Startup SoundBanks")] //Liste de nos soundbanks
        [SerializeField] private List<AK.Wwise.Bank> Soundbanks;
        #endregion

        #region GameState variables
        [Header("Game States")]
        [SerializeField] private AK.Wwise.State Game_Gameplay;
        [SerializeField] private AK.Wwise.State Game_MainMenu;
        [SerializeField] private AK.Wwise.State Game_Paused;
        [SerializeField] private AK.Wwise.State Game_GameOver;
        [SerializeField] private AK.Wwise.State Game_None;

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
            SetAudioGameState(AudioGameState.None); //On initialise l'état du jeu à None (reset)
            SetAudioMusicState(AudioMusicState.None); //On initialise l'état de la musique à None (reset)  
        }

        void Start()
        {
            SetAudioGameState(AudioGameState.MainMenu); //On initialise l'état du jeu à MainMenu
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

        void LoadSoundbanks() //Load les soundbanks (pas encore dynamiquement)
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

        public void SetAudioGameState(AudioGameState GameState) // Change l'état des states liés au jeu
        {
            if (GameState == currentGameState) //Si c'est la même valeur que celle actuelle, on ne fait rien
            {
                Debug.Log("GameState is already" + GameState + "."); //On ne change pas l'état si c'est déjà le bon
                return;
            }
            switch(GameState) // On change l'état en fonction de l'état du jeu
            {
                default: //Cas pas défaut = mainmenu
                case AudioGameState.MainMenu:
                    Game_MainMenu.SetValue();
                    break;
                case AudioGameState.Gameplay:
                    Game_Gameplay.SetValue();
                    break;
                case AudioGameState.Paused:
                    Game_Paused.SetValue();
                    break;
                case AudioGameState.GameOver:
                    Game_GameOver.SetValue();
                    break;
                case AudioGameState.None:   
                    Game_None.SetValue();
                    break;
            }
            Debug.Log("New Wwise GameState: " + GameState + "."); //On affiche le nouvel état dans la console

            currentGameState = GameState; //On met à jour l'état actuel
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
  
        public void PostWwiseEventGlobal(AK.Wwise.Event WwiseEvent)
        {
            if (WwiseEvent == null)
            {
                Debug.LogError(WwiseEvent.Name + " is null (check if it's set correctly and up to date :)");
                return;
            }
            
            if (WwiseEvent.IsValid())
            {
                WwiseEvent.Post(gameObject);
            }
            else
            {
                Debug.LogError(WwiseEvent.Name + " is invalid, check if it's set correctly and up to date");
            }
        }

        public void PostWwiseEventToObject(AK.Wwise.Event WwiseEvent, GameObject TargetObject)
        {
            if (WwiseEvent == null)
            {
                Debug.LogError(WwiseEvent.Name + " is null (check if it's set correctly and up to date :)");
                return;
            }
            else if(TargetObject == null)
            {
                Debug.LogError(TargetObject.name + " is null. PostWwiseEventToObject requires an existing TargetObject.");
                return;
            }

            if (WwiseEvent.IsValid())
            {
                WwiseEvent.Post(TargetObject);
            }
            else
            {
                Debug.LogError(WwiseEvent.Name + " is invalid, check if it's set correctly and up to date");
            }

            
        }

    }



}
