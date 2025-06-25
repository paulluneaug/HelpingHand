using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.Filters;
using UnityUtility.MathU;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class FilterNoiseProcessor : InputProcessor<float>
{
    public float MinCutoff = 1.0f;
    public float Beta = 0.007f;
    public float MinChangeRate = 0.05f;

    private OneEuroFilter m_filter;
    private float m_previousTime;
    private float m_changeRate;
    private float m_previousValue;

#if UNITY_EDITOR
    static FilterNoiseProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        InputSystem.RegisterProcessor<FilterNoiseProcessor>();
    }

    public override float Process(float value, InputControl control)
    {
        m_filter ??= new OneEuroFilter(MinCutoff, Beta);
        float deltaTime = Time.time - m_previousTime;
        if (deltaTime <= 0.0f)
        {
            return m_previousValue;
        }
        m_previousTime = Time.time;
        float filteredValue = m_filter.Filter(value, deltaTime);
        float changeRate = MathUf.Abs(filteredValue - m_previousValue) / deltaTime;
        if (changeRate < MinChangeRate)
        {
            return m_previousValue;
        }
        m_previousValue = filteredValue;
        return filteredValue;
    }

}
