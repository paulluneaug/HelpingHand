using UnityEditor.SceneManagement;

public static class LoadLevelUtils
{
    public static void LoadLevel(string levelScenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            StartupScenesReferences startupScenesReferences = StartupScenesReferences.Instance;

            _ = EditorSceneManager.OpenScene(startupScenesReferences.EntryScene, OpenSceneMode.Single);
            _ = EditorSceneManager.OpenScene(startupScenesReferences.GloabalObjectsScene, OpenSceneMode.Additive);
            _ = EditorSceneManager.OpenScene(levelScenePath, OpenSceneMode.Additive);
        }
    }
}
