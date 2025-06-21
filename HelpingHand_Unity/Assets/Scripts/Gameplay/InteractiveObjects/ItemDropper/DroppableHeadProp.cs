using UnityEngine;

public class DroppableHeadProp : DroppableItem
{
    public ObjectOnHead ObjectType => m_objectType;

    [SerializeField] private ObjectOnHead m_objectType;

    public override void DropItem()
    {
        throw new System.NotImplementedException();
    }
}
