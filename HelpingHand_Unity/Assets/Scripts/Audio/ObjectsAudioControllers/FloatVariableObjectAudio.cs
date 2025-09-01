using Sirenix.OdinInspector;

using UnityEngine;

public class FloatVariableObjectAudio : SerializedMonoBehaviour
{
    [SerializeField] private IBaseVariableContainer<float> m_variableContainer;
    [SerializeField] private FloatObjectAudioContainer m_audioContainer;

    private void Start()
    {
        m_audioContainer.Init(m_variableContainer.Variable, gameObject);
    }

    private void Update()
    {
        m_audioContainer.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        m_audioContainer.Dispose();
    }
}
