using UnityEngine;

public abstract class VirtualInput<T> : MonoBehaviour, IVirtualInput
{
    public abstract T Value { get; }
}
