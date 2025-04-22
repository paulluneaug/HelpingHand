using System;

using UnityEditor;

using UnityEngine;

using UnityUtility.Extensions;

[CustomEditor(typeof(InteractiveObjectTest))]
public class InteractiveObjectTestEditor : Editor
{
    [NonSerialized] private Tool m_currentTool;

    private GUIStyle m_style;
    
    private void Awake()
    {
        m_currentTool = Tools.current;
        Tools.current = Tool.None;
    }

    private GUIStyle GetStyle()
    {
        GUIStyle style = new GUIStyle("ProfilerBadge");
        style.alignment = TextAnchor.UpperLeft;
        var padding = style.padding;
        padding.left = 4;
        padding.right = 0;
        style.padding = padding;
        return style;
    }

    protected virtual void OnSceneGUI()
    {
        if (m_style == null)
        {
            m_style = GetStyle();
        }
        
        InteractiveObjectTest interactiveObject = (InteractiveObjectTest)target;

        float gridSize = PuppetSettings.Instance.TileSize;

        EditorGUI.BeginChangeCheck();

        Vector3 newStartPosition = Handles.PositionHandle(interactiveObject.StartPosition, Quaternion.identity).Snap(gridSize);
        Handles.Label(newStartPosition + new Vector3(.05f, -0.05f, 0), new GUIContent("Min"), m_style);
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(interactiveObject, "Change Start Position");
            interactiveObject.StartPosition = newStartPosition;
            interactiveObject.transform.position = newStartPosition;
        }

        EditorGUI.BeginChangeCheck();
        
        Vector3 newEndPosition = Handles.PositionHandle(interactiveObject.EndPosition, Quaternion.identity).Snap(gridSize);
        Handles.Label(newEndPosition+ new Vector3(.05f, -0.05f, 0), new GUIContent("Max"), m_style);
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(interactiveObject, "Change End Position");
            interactiveObject.EndPosition = newEndPosition;
        }
    }

    private void OnDestroy()
    {
        Tools.current = m_currentTool;
    }
}
