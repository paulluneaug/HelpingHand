using UnityEngine;

using UnityUtility.CustomAttributes;

using WwiseEvent = AK.Wwise.Event;
using WwiseState = AK.Wwise.State;

public class MusicManager : MonoBehaviour
{
    #region GameState variables

    [Title("Music States")]
    [SerializeField] private WwiseState m_music_Gameplay1stSection;
    [SerializeField] private WwiseState m_music_Gameplay2ndSection;
    [SerializeField] private WwiseState m_music_Gameplay3rdSection;
    [SerializeField] private WwiseState m_music_MainMenu;
    [SerializeField] private WwiseState m_music_Level_Lose;
    [SerializeField] private WwiseState m_music_Level_Win;
    [SerializeField] private WwiseState m_music_None;

    [Title("RTPC")]
    [SerializeField, Range(0f, 1f)] private float m_music_FirstLayer = 0;
    [SerializeField, Range(0f, 1f)] private float m_music_SecondLayer = 0;
    #endregion


    public void PostWwiseEventGlobal(WwiseEvent WwiseEvent)
    {
        if (WwiseEvent == null)
        {
            Debug.LogError(WwiseEvent.Name + " is null (check if it's set correctly and up to date :)");
            return;
        }

        if (WwiseEvent.IsValid())
        {
            _ = WwiseEvent.Post(gameObject);
        }
        else
        {
            Debug.LogError(WwiseEvent.Name + " is invalid, check if it's set correctly and up to date");
        }
    }

    public void PostWwiseEventToObject(WwiseEvent WwiseEvent, GameObject TargetObject)
    {
        if (WwiseEvent == null)
        {
            Debug.LogError(WwiseEvent.Name + " is null (check if it's set correctly and up to date :)");
            return;
        }
        else if (TargetObject == null)
        {
            Debug.LogError(TargetObject.name + " is null. PostWwiseEventToObject requires an existing TargetObject.");
            return;
        }

        if (WwiseEvent.IsValid())
        {
            _ = WwiseEvent.Post(TargetObject);
        }
        else
        {
            Debug.LogError(WwiseEvent.Name + " is invalid, check if it's set correctly and up to date");
        }


    }


}
