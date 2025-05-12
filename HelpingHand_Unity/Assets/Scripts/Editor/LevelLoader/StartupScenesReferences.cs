using UnityEngine;

using UnityUtility.SceneReference;
using UnityUtility.Singletons;

[CreateAssetMenu(fileName = "StartupScenesReferences", menuName = "Scriptable Objects/StartupScenesReferences")]
public class StartupScenesReferences : ScriptableSingleton<StartupScenesReferences>
{
    public SceneReference EntryScene => m_entryScene;

    [SerializeField] private SceneReference m_entryScene;
}
