using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.Extensions;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class RemapProcessor : InputProcessor<float>
{
    public float InitialRangeX;
    public float InitialRangeY;
    public float TargetRangeX = 0.0f;
    public float TargetRangeY = 1.0f;

#if UNITY_EDITOR
    static RemapProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        InputSystem.RegisterProcessor<RemapProcessor>();
    }

    public override float Process(float value, InputControl control)
    {
        return value.Remap(InitialRangeX, InitialRangeY, TargetRangeX, TargetRangeY);
    }

}
