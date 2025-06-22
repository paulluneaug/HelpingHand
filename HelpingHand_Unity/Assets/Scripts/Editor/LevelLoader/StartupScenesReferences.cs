using UnityEngine;

using UnityUtility.SceneReference;
using UnityUtility.Singletons;

[CreateAssetMenu(fileName = "StartupScenesReferences", menuName = "Scriptable Objects/StartupScenesReferences")]
public class StartupScenesReferences : ScriptableSingleton<StartupScenesReferences>
{
    public SceneReference EntryScene => m_entryScene;
    public SceneReference GloabalObjectsScene => m_gloabalObjectsScene;
    public SceneReference VirtualControllerScene => m_virtualControllerScene;

    [SerializeField] private SceneReference m_entryScene;
    [SerializeField] private SceneReference m_gloabalObjectsScene;
    [SerializeField] private SceneReference m_virtualControllerScene;
}
