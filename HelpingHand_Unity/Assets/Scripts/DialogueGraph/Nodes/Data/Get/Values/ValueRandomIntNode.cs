using System;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

[CreateNodeMenu("Data/Get/Values/Random Int")]
public class ValueRandomIntNode : ValueNodeBase<int>
{
    [Output(ShowBackingValue.Never)]
    [SerializeField]
    private int m_value;

    [SerializeField]
    private int m_rangeMin;

    [SerializeField]
    private int m_rangeMax;

    [NonSerialized] 
    [ShowInInspector] 
    [ReadOnly]
    private int m_currentValue;

    protected override int Value
    {
        get
        {
            m_currentValue = Random.Range(m_rangeMin, m_rangeMax);
            return m_currentValue;
        }
    }
}