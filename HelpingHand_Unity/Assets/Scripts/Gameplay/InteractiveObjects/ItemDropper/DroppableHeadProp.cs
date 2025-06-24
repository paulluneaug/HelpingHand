using UnityEngine;

public class DroppableHeadProp : DroppableItem
{
    public ObjectOnHead ObjectType => m_objectType;

    [SerializeField] private ObjectOnHead m_objectType;

    public override void DropItem()
    {
        ActivateModel();
        GameManager.Instance.GetPuppet().WearObjectOnHead(this);
    }

    public override bool CanDrop()
    {
        return !GameManager.Instance.GetPuppet().IsWearingObjectOnHead();
    }
}
