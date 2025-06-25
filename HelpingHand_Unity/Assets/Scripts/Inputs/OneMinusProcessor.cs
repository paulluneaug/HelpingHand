using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.Extensions;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class OneMinusProcessor : InputProcessor<float>
{

#if UNITY_EDITOR
    static OneMinusProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        InputSystem.RegisterProcessor<OneMinusProcessor>();
    }

    public override float Process(float value, InputControl control)
    {
        return 1.0f - value;
    }

}
