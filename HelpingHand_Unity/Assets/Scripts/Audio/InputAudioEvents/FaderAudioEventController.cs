using UnityEngine;

public class FaderAudioEventController : MonoBehaviour
{
    [SerializeField] private FloatInputEvent m_faderVariable;
    [SerializeField] private FloatObjectAudioContainer m_audioContainer;


    public void Init()
    {
        m_audioContainer.Init(m_faderVariable, gameObject);
    }

    private void Update()
    {
        m_audioContainer.Update(Time.deltaTime);
    }

    public void Dispose()
    {
        m_audioContainer.Dispose();
    }
}
