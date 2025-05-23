using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public InputActionTriggersManager InputActionTriggersManager => m_inputActionTriggersManager;
    public LevelSequenceManager LevelSequenceManager => m_levelSequenceManager;

    [Title("Sub Managers", titleAlignment: TitleAlignments.Centered)]

    [SerializeField, Label(bold: true)] private InputActionTriggersManager m_inputActionTriggersManager;
    [Separator]
    [SerializeField, Label(bold: true)] private LevelSequenceManager m_levelSequenceManager;


    public override void Initialize()
    {
        base.Initialize();
        m_inputActionTriggersManager.Initialize();
    }

    private void Update()
    {
        m_inputActionTriggersManager.Update();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_inputActionTriggersManager.Dispose();
    }

}
