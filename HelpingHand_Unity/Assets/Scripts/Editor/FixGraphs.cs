using UnityEditor;

using UnityEngine;

public static class FixGraphs
{
    [MenuItem("Assets/Fix graph", true)]
    private static bool FixGraphValidation()
    {
        return Selection.activeObject is SimpleGraph;
    }
    
    [MenuItem("Assets/Fix graph")]
    private static void FixGraph()
    {
        SimpleGraph selected = Selection.activeObject as SimpleGraph;
        if (selected == null)
        {
            return;
        }
        
        //Create a new instance of the object to delete
        ScriptableObject newInstance = ScriptableObject.CreateInstance(selected.GetType());

        //Copy the original content to the new instance
        EditorUtility.CopySerialized(selected, newInstance);
        newInstance.name = selected.name;

        string selectedPath = AssetDatabase.GetAssetPath(selected);
        string clonePath = selectedPath.Replace(".asset", "CLONE.asset");

        //Create the new asset on the project files
        AssetDatabase.CreateAsset(newInstance, clonePath);
        AssetDatabase.ImportAsset(clonePath);

        //Unhide sub-assets
        var subAssets = AssetDatabase.LoadAllAssetsAtPath(selectedPath);
        HideFlags[] flags = new HideFlags[subAssets.Length];
        for (int i = 0; i < subAssets.Length; i++)
        {
            //Ignore the "corrupt" one
            if (subAssets[i] == null)
                continue;

            //Store the previous hide flag
            flags[i] = subAssets[i].hideFlags;
            subAssets[i].hideFlags = HideFlags.None;
            EditorUtility.SetDirty(subAssets[i]);
        }

        EditorUtility.SetDirty(selected);
        AssetDatabase.SaveAssets();

        //Reparent the subAssets to the new instance
        foreach (var subAsset in AssetDatabase.LoadAllAssetRepresentationsAtPath(selectedPath))
        {
            //Ignore the "corrupt" one
            if (subAsset == null)
                continue;

            //We need to remove the parent before setting a new one
            AssetDatabase.RemoveObjectFromAsset(subAsset);
            AssetDatabase.AddObjectToAsset(subAsset, newInstance);
        }

        //Import both assets back to unity
        AssetDatabase.ImportAsset(selectedPath);
        AssetDatabase.ImportAsset(clonePath);

        //Reset sub-asset flags
        for (int i = 0; i < subAssets.Length; i++)
        {
            //Ignore the "corrupt" one
            if (subAssets[i] == null)
                continue;

            subAssets[i].hideFlags = flags[i];
            EditorUtility.SetDirty(subAssets[i]);
        }

        EditorUtility.SetDirty(newInstance);
        AssetDatabase.SaveAssets();

        //Here's the magic. First, we need the system path of the assets
        string globalselectedPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), selectedPath);
        string globalClonePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), clonePath);

        //We need to delete the original file (the one with the missing script asset)
        //Rename the clone to the original file and finally
        //Delete the meta file from the clone since it no longer exists

        System.IO.File.Delete(globalselectedPath);
        System.IO.File.Delete(globalClonePath + ".meta");
        System.IO.File.Move(globalClonePath, globalselectedPath);

        AssetDatabase.Refresh();
    }
}