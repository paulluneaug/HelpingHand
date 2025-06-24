using UnityEngine;
using UnityEngine.UI;

public static class ScrollRectExtensions
{
    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = instance.content.InverseTransformPoint(child.position);
        Vector2 result = new Vector2(
            -(viewportLocalPosition.x + childLocalPosition.x),
            -(viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }
}