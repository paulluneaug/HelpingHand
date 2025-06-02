using UnityEngine.UI;

public static class NavigationExtensions
{

    public static Navigation Clone(this Navigation navigation)
    {
        return new Navigation
        {
            selectOnUp = navigation.selectOnUp,
            selectOnDown = navigation.selectOnDown,
            selectOnLeft = navigation.selectOnLeft,
            selectOnRight = navigation.selectOnRight,
            mode = navigation.mode,
            wrapAround = navigation.wrapAround
        };
    }
}
