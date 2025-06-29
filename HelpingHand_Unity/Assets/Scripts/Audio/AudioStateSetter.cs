using UnityEngine;
using AK.Wwise;

public class AudioStateSetter : MonoBehaviour
{
    [SerializeField] private State m_audioState;

    public void SetState()
    {
        m_audioState.SetValue();
    }
}