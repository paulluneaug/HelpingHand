using System;

public class VirtualButton : VirtualInput<bool>
{
    public override bool Value => m_value;

    [NonSerialized] private readonly bool m_value;
}
