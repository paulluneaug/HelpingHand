using UnityEngine;

public class InputEventsRaiserManager : MonoBehaviour
{
    [SerializeField] private PotentiometerEventRaiser[] m_faders;
    [SerializeField] private ButtonsEventRaiser[] m_buttonsEventRaiser;
}
