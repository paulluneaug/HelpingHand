using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class ArduinoDueDevice
{
    static ArduinoDueDevice()
    {
        string json = Resources.Load<TextAsset>("ArduinoDue_Layout").text;

        //InputSystem.RegisterLayout(json);
        //var device = InputSystem.AddDevice("ArduinoDue_Custom");
    }
}
