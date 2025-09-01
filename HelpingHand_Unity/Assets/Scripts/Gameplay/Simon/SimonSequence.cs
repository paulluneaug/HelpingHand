using System;

using UnityEngine;

[Serializable]
public class SimonSequence
{
    public enum SimonColor : byte
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        White = 3,
    }

    public SimonColor[] Sequence => m_sequence;

    [SerializeField] private SimonColor[] m_sequence;
}
