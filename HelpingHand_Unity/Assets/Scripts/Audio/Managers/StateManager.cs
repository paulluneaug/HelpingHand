using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.CustomAttributes;

using WwiseState = AK.Wwise.State;

#region Enums States
public enum GameState
{
    None,
    MainMenu,
    Gameplay,
    Paused,
    GameOver
}
public enum MusicState
{
    None,
    MainMenu,
    PauseMenu,
    LevelWin,
    LevelLose,
    Onboarding_1,
    Onboarding_2,
    Onboarding_3,
    BattleTheme,
    SadTheme,
    SuspenseTheme,
    HorribleMusicTheme
}
#endregion

public class StateManager : MonoBehaviour
{
    #region Game States
    // [TitleGroup("States", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: true)]
    [FoldoutGroup("Game States")]
    [InfoBox("➔<b>SetGameState(GameState.MainMenu)</b>", InfoMessageType.None)]
    [FoldoutGroup("Game States")][LabelWidth(200)][SerializeField] private WwiseState m_game_Paused;
    [FoldoutGroup("Game States")][LabelWidth(200)][SerializeField] private WwiseState m_game_MainMenu;
    [FoldoutGroup("Game States")][LabelWidth(200)][SerializeField] private WwiseState m_game_None;
    [FoldoutGroup("Game States")][LabelWidth(200)][SerializeField] private WwiseState m_game_GameOver;
    [FoldoutGroup("Game States")][LabelWidth(200)][SerializeField] private WwiseState m_game_Gameplay;
    [FoldoutGroup("Game States")][LabelWidth(200)][SerializeField] private WwiseState m_game_Win;
    [Disable][SerializeField] public GameState CurrentGameState;
    #endregion
    #region Music States
    [FoldoutGroup("Music States")]
    [InfoBox("➔<b>SetMusicState(MusicState.MainMenu)</b>", InfoMessageType.None)]
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_Unboarding_1;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_Unboarding_2;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_Unboarding_3;

    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_MainMenu;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_LevelLose;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_LevelWin;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_None;

    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_BattleTheme;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_SadTheme;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_SuspenseTheme;

    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_HorribleMusicTheme;
    [Disable][SerializeField] public MusicState CurrentMusicState;
    #endregion

    public void SetGameState(GameState GameState) // Change l'état des states liés au jeu
    {
        if (GameState == CurrentGameState) //Si c'est la même valeur que celle actuelle, on ne fait rien
        {
            //Debug.Log("GameState is already" + GameState + "."); //On ne change pas l'état si c'est déjà le bon
            return;
        }
        switch (GameState) // On change l'état en fonction de l'état du jeu
        {
            default: //Cas pas défaut = mainmenu
            case GameState.MainMenu:
                m_game_MainMenu.SetValue();
                break;
            case GameState.Gameplay:
                m_game_Gameplay.SetValue();
                break;
            case GameState.Paused:
                m_game_Paused.SetValue();
                break;
            case GameState.GameOver:
                m_game_GameOver.SetValue();
                break;
            case GameState.None:
                m_game_None.SetValue();
                break;
        }
        //Debug.Log("New Wwise GameState: " + GameState + "."); //On affiche le nouvel état dans la console

        CurrentGameState = GameState; //On met à jour l'état actuel
    }

    public void SetMusicState(MusicState MusicState)
    {
        if (MusicState == CurrentMusicState)
            return;

        switch (MusicState)
        {
            case MusicState.MainMenu:
                m_music_MainMenu.SetValue();
                break;

            case MusicState.LevelWin:
                m_music_LevelWin.SetValue(); // À remplacer si tu as un m_music_LevelWin
                break;

            case MusicState.LevelLose:
                m_music_LevelLose.SetValue();
                break;

            case MusicState.Onboarding_1:
                m_music_Unboarding_1.SetValue();
                break;

            case MusicState.Onboarding_2:
                m_music_Unboarding_2.SetValue();
                break;

            case MusicState.Onboarding_3:
                m_music_Unboarding_3.SetValue();
                break;

            case MusicState.BattleTheme:
                m_music_BattleTheme.SetValue();
                break;

            case MusicState.SadTheme:
                m_music_SadTheme.SetValue();
                break;

            case MusicState.SuspenseTheme:
                m_music_SuspenseTheme.SetValue();
                break;

            case MusicState.HorribleMusicTheme:
                m_music_HorribleMusicTheme.SetValue();
                break;

            case MusicState.None:
            default:
                m_music_None.SetValue();
                break;
        }

        CurrentMusicState = MusicState;
    }


    //Debug.Log("New Wwise GameState: " + MusicState + ".");
}
