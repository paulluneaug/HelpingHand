using UnityEngine;
using UnityEditor;

public static class EditorUtils
{
    [MenuItem("Tools/Reset PlayerPrefs")]
    public static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
