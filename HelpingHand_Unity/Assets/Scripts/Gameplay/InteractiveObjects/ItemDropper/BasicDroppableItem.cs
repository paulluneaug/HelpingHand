using UnityEngine;

public class BasicDroppableItem : DroppableItem
{
    public override void DropItem()
    {
        Debug.LogWarning($"Dropping {name}");
    }
}
