using System;

using UnityEngine;

[Serializable]
public class PuppetObjectOnHeadStateCollection
{
    [SerializeField] private EntityState m_helmetOnHeadState;
    [SerializeField] private EntityState m_bunnyEarsOnHeadState;
    [SerializeField] private EntityState m_chickenHatOnHeadState;
    [SerializeField] private EntityState m_headphonesOnHeadState;
    [SerializeField] private EntityState m_beretOnHeadState;

    public void OnPuppetWornObjectChanged(ObjectOnHead wornObject)
    {
        m_helmetOnHeadState.Value = wornObject == ObjectOnHead.Helmet;
        m_bunnyEarsOnHeadState.Value = wornObject == ObjectOnHead.BunnyEars;
        m_chickenHatOnHeadState.Value = wornObject == ObjectOnHead.ChickenHat;
        m_headphonesOnHeadState.Value = wornObject == ObjectOnHead.Headphone;
        m_beretOnHeadState.Value = wornObject == ObjectOnHead.Beret;
    }
}
