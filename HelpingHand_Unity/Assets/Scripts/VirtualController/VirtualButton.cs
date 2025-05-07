using System;

using UnityEngine;
using UnityEngine.UI;

public class VirtualButton : VirtualInput<bool>
{
    public override bool Value => m_value;

    [SerializeField] private Button m_button;

    [NonSerialized] private readonly bool m_value;

    public override event Action<bool> OnValueChanged;
}
