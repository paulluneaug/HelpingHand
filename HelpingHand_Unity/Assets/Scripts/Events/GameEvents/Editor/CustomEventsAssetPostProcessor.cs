using UnityEditor;

using UnityEngine;

public class CustomEventsAssetPostProcessor : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        foreach (string assetPath in movedAssets)
        {
            Object rootAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (rootAsset is RotaryEncoderInputEvent rotaryEncoderEvent)
            {
                Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                foreach (Object subAsset in subAssets)
                {
                    string suffix = subAsset.name.Contains("OnStepLeftEvent") ? "OnStepLeftEvent" : subAsset.name.Contains("OnStepRightEvent") ? "OnStepRightEvent" : "Variable_Index";
                    subAsset.name = $"{rootAsset.name}_{suffix}";
                }
            }
            else if (rootAsset is ButtonInputEvent buttonInputEvent)
            {
                Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                foreach (Object subAsset in subAssets)
                {
                    string suffix = subAsset.name.Contains("OnDownEvent") ? "OnDownEvent" : subAsset.name.Contains("OnUpEvent") ? "OnUpEvent" : "State";
                    subAsset.name = $"{rootAsset.name}_{suffix}";
                }
            }
            AssetDatabase.SaveAssetIfDirty(rootAsset);
        }
    }
}
