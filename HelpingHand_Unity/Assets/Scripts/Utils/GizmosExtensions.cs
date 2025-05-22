using UnityEngine;

using UnityUtility.MathU;

public static class GizmosExtensions
{
    public static void DrawCircle(Vector3 center, float radius, Vector3 normal, int resolution = 32)
    {
        Matrix4x4 previousMatrix = Gizmos.matrix;

        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, normal);
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);

        float step = MathUf.TAU / resolution;

        Vector3 previousPoint = GetPointOnCircle(Vector3.zero, radius, 0.0f);
        for (int i = 1; i <= resolution; ++i)
        {
            Vector3 newPoint = GetPointOnCircle(Vector3.zero, radius, i * step);

            Gizmos.DrawLine(previousPoint, newPoint);

            previousPoint = newPoint;
        }

        Gizmos.matrix = previousMatrix;
    }

    public static void DrawConeFromBase(Vector3 baseCenter, Vector3 tip, float radius)
    {
        Matrix4x4 previousMatrix = Gizmos.matrix;

        Vector3 normal = baseCenter - tip;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, normal);
        Gizmos.matrix = Matrix4x4.TRS(baseCenter, rotation, Vector3.one);

        DrawCircle(baseCenter, radius, normal, 32);


        int frustrumLinesCount = 4;
        float step = MathUf.TAU / frustrumLinesCount;

        Vector3 inversedTip = Gizmos.matrix.inverse.MultiplyPoint(tip);

        for (int i = 0; i < frustrumLinesCount; ++i)
        {
            Vector3 pointOnCircle = GetPointOnCircle(Vector3.zero, radius, i * step);
            Gizmos.DrawLine(pointOnCircle, inversedTip);
        }

        Gizmos.matrix = previousMatrix;
    }

    public static void DrawConeFromRadius(Vector3 tip, Vector3 direction, float radius, float height)
    {
        Vector3 baseCenter = tip + direction * height;
        DrawConeFromBase(baseCenter, tip, radius);
    }

    public static void DrawConeFromAngle(Vector3 tip, Vector3 direction, float height, float angle)
    {
        Vector3 baseCenter = tip + direction * height;
        float radius = height * MathUf.Tan(angle / 2.0f);
        DrawConeFromBase(baseCenter, tip, radius);
    }

    private static Vector3 GetPointOnCircle(Vector3 center, float radius, float angle)
    {
        return new Vector3(
            center.x + MathUf.Cos(angle) * radius,
            center.y + MathUf.Sin(angle) * radius,
            center.z);
    }
}
