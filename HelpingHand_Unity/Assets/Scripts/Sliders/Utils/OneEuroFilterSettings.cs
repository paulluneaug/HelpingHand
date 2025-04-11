using System;

using UnityEngine;

[Serializable]
public class OneEuroFilterSettings
{
    public float MinCutoff => m_minCutoff;
    public float Beta => m_beta;


    [SerializeField] private float m_minCutoff = 1.0f;
    [SerializeField] private float m_beta = 0.007f;

}
