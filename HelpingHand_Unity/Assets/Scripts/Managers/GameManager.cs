using System;

using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private enum GameState
    {
        MainMenu,
        Game,
    }

    public SlidersManager SlidersManager => m_sliderManager;
    public LevelSequenceManager LevelSequenceManager => m_levelSequenceManager;
    public GameOptionsManager GameOptionsManager => m_gameOptionsManager;

    [Title("Sub Managers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Label(bold: true)] private SlidersManager m_sliderManager;
    [Separator]
    [SerializeField, Label(bold: true)] private LevelSequenceManager m_levelSequenceManager;
    [Separator]
    [SerializeField, Label(bold: true)] private GameOptionsManager m_gameOptionsManager;

    [Title("Puppet")]
    [SerializeField] private Puppet m_puppet;

    [NonSerialized] private readonly GameState m_currentGameState;

    public override void Initialize()
    {
        base.Initialize();
        m_levelSequenceManager.Initialize(m_puppet);

    }

    protected override void Start()
    {
        base.Start();
        m_levelSequenceManager.Start();
    }

    private void Update()
    {
        m_levelSequenceManager.Update(Time.deltaTime);
    }

    public void StartGame()
    {
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
