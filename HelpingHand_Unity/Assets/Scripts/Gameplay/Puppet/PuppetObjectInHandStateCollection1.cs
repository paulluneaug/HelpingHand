using System;

using UnityEngine;

[Serializable]
public class PuppetObjectInHandStateCollection
{
    [SerializeField] private EntityState m_swordInHandState;
    [SerializeField] private EntityState m_broomInHandState;
    [SerializeField] private EntityState m_carrotInHandState;
    [SerializeField] private EntityState m_mugInHandState;
    [SerializeField] private EntityState m_baguetteInHandState;

    public void OnPuppetHeldObjectChanged(ObjectInHand heldObject)
    {
        m_swordInHandState.Value = heldObject == ObjectInHand.Sword;
        m_broomInHandState.Value = heldObject == ObjectInHand.Broom;
        m_carrotInHandState.Value = heldObject == ObjectInHand.Carrot;
        m_mugInHandState.Value = heldObject == ObjectInHand.Mug;
        m_baguetteInHandState.Value = heldObject == ObjectInHand.Baguette;
    }
}
