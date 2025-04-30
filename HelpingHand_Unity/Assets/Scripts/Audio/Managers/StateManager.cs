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
    GameplayFirstSection,
    GameplaySecondSection,
    GameplayThirdSection,
    PauseMenu,
    LevelWin,
    LevelLose
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
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_Gameplay1stSection;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_Gameplay2ndSection;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_Gameplay3rdSection;

    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_MainMenu;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_Level_Lose;
    [FoldoutGroup("Music States")][LabelWidth(200)][SerializeField] private WwiseState m_music_None;
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

    public void SetMusicState(MusicState MusicState) // Change l'état des states liés à la musique
    {
        if (MusicState == CurrentMusicState)
        {
            //Debug.Log("MusicState is already" + MusicState + "."); //On ne change pas l'état si c'est déjà le bon
            return;
        }

        switch (MusicState)
        {
            default: //Cas pas défaut = mainmenu
            case MusicState.MainMenu:
                m_music_MainMenu.SetValue();
                break;
            case MusicState.GameplayFirstSection:
                m_music_Gameplay1stSection.SetValue();
                break;
            case MusicState.LevelLose:
                m_music_Level_Lose.SetValue();
                break;
            case MusicState.None:
                m_music_None.SetValue();
                break;
            case MusicState.GameplaySecondSection:
                break;
            case MusicState.GameplayThirdSection:
                break;
            case MusicState.PauseMenu:
                break;
            case MusicState.LevelWin:
                break;
        }
        CurrentMusicState = MusicState; //On met à jour l'état actuel
        //Debug.Log("New Wwise GameState: " + MusicState + ".");
    }
}
