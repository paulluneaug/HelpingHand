using UnityEditor;

using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        _ = EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}