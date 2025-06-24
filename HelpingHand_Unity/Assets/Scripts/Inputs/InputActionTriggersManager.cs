using System;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.Extensions;

[Serializable]
public class InputActionTriggersManager : SerializedMonoBehaviour
{
    [SerializeField, DontFold] private InputActionTrigger<bool>[] m_boolInputTriggers;
    [SerializeField] private InputActionTrigger<float>[] m_floatInputTriggers;
    [SerializeField] private InputActionTrigger<int>[] m_intInputTriggers;
    [SerializeField] private InputActionTrigger<Vector2>[] m_vector2InputTriggers;

    private void Awake()
    {
        m_boolInputTriggers.ForEach(trigger => trigger.Initialize());
        m_floatInputTriggers.ForEach(trigger => trigger.Initialize());
        m_intInputTriggers.ForEach(trigger => trigger.Initialize());
        m_vector2InputTriggers.ForEach(trigger => trigger.Initialize());
    }

    private void Update()
    {
        m_boolInputTriggers.ForEach(trigger => trigger.Update());
        m_floatInputTriggers.ForEach(trigger => trigger.Update());
        m_intInputTriggers.ForEach(trigger => trigger.Update());
        m_vector2InputTriggers.ForEach(trigger => trigger.Update());
    }

    private void OnDestroy()
    {
        m_boolInputTriggers.ForEach(trigger => trigger.Dispose());
        m_floatInputTriggers.ForEach(trigger => trigger.Dispose());
        m_intInputTriggers.ForEach(trigger => trigger.Dispose());
        m_vector2InputTriggers.ForEach(trigger => trigger.Dispose());
    }
}
