using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public SlidersManager SlidersManager => m_sliderManager;

    [Title("Sub Managers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Label(bold: true)] private SlidersManager m_sliderManager;
}
