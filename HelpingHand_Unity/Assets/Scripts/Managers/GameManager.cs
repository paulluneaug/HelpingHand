using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public SlidersManager SlidersManager => m_sliderManager;
    public LevelSequenceManager LevelSequenceManager => m_levelSequenceManager;

    [Title("Sub Managers", titleAlignment: TitleAlignments.Centered)]
    [SerializeField, Label(bold: true)] private SlidersManager m_sliderManager;
    [Separator]
    [SerializeField, Label(bold: true)] private LevelSequenceManager m_levelSequenceManager;

    [Title("Puppet")]
    [SerializeField] private Puppet m_puppet;


    public override void Initialize()
    {
        base.Initialize();
        m_levelSequenceManager.Initialize(m_puppet);
    }

    protected override void Start()
    {
        base.Start();
        // m_levelSequenceManager.Start();
    }

    private void Update()
    {
        // m_levelSequenceManager.Update(Time.deltaTime);
    }

}
