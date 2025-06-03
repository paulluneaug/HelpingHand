using UnityEngine;

using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;

public class InputAudioEventControllersManager : MonoBehaviour
{
    [Title("Faders and potentiometers")]
    [SerializeField] private FaderAudioEventController[] m_fadersAudioEventControllers;

    [Title("Buttons")]
    [SerializeField] private ButtonAudioEventController[] m_buttonsAudioEventControllers;

    [Title("Toggles")]
    [SerializeField] private ToggleAudioEventController[] m_toggleAudioEventControllers;

    [Title("Rotary Encoders")]
    [SerializeField] private RotaryEncoderAudioEventController[] m_rotaryEncoderAudioEventControllers;

    public void Init()
    {
        m_fadersAudioEventControllers.ForEach(fader => fader.Init());
        m_buttonsAudioEventControllers.ForEach(button => button.Init(gameObject));
        m_toggleAudioEventControllers.ForEach(toggle => toggle.Init(gameObject));
        m_rotaryEncoderAudioEventControllers.ForEach(rotary => rotary.Init(gameObject));
    }

    public void Dispose()
    {
        m_fadersAudioEventControllers.ForEach(fader => fader.Dispose());
        m_buttonsAudioEventControllers.ForEach(button => button.Dispose());
        m_toggleAudioEventControllers.ForEach(toggle => toggle.Dispose());
        m_rotaryEncoderAudioEventControllers.ForEach(rotary => rotary.Dispose());
    }
}
