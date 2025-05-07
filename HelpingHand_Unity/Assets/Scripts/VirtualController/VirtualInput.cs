using System;

using UnityEngine;

public abstract class VirtualInput<T> : MonoBehaviour, IVirtualInput
{
    public abstract T Value { get; }
    public abstract event Action<T> OnValueChanged;
}
