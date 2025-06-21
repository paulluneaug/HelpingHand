using UnityEngine;

public class DroppableHandProp : DroppableItem
{
    public ObjectInHand ObjectType => m_objectType;

    [SerializeField] private ObjectInHand m_objectType;


    public override void DropItem()
    {
        throw new System.NotImplementedException();
    }
}
