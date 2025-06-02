using System;

using UnityEngine;

[Serializable]
public class ButtonsEventRaiser
{
    [SerializeField] private BaseVariable<bool> m_buttonVariables;
    [SerializeField] private AK.Wwise.Event m_pointerUpEvent;
    [SerializeField] private AK.Wwise.Event m_pointerDownEvent;


}
