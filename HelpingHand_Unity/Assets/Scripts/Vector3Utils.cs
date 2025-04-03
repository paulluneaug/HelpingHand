using UnityEngine;

public static class Vector3Utils
{
    public static Vector3 WhereX(this Vector3 v, float xValue)
    {
        v.x = xValue;
        return v;
    }
    public static Vector3 WhereY(this Vector3 v, float yValue)
    {
        v.y = yValue;
        return v;
    }
}
