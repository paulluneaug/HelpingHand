using System;

public class VirtualToggle : VirtualInput<bool>
{
    public override bool Value => m_value;

    [NonSerialized] private readonly bool m_value;
}
