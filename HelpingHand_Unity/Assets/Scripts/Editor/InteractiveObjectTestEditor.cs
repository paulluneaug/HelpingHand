using System;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(InteractiveObjectTest))]
public class InteractiveObjectTestEditor : Editor
{
    [NonSerialized] private Tool m_currentTool;

    private void Awake()
    {
        m_currentTool = Tools.current;
        Tools.current = Tool.None;
    }

    protected virtual void OnSceneGUI()
    {
        InteractiveObjectTest interactiveObject = (InteractiveObjectTest)target;

        EditorGUI.BeginChangeCheck();
        Vector3 newStartPosition = Handles.PositionHandle(interactiveObject.StartPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(interactiveObject, "Change Start Position");
            interactiveObject.StartPosition = newStartPosition;
            interactiveObject.transform.position = newStartPosition;
        }

        EditorGUI.BeginChangeCheck();
        Vector3 newEndPosition = Handles.PositionHandle(interactiveObject.EndPosition, Quaternion.identity);
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
