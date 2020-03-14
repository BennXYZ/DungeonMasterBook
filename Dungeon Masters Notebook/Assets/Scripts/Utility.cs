
using UnityEngine;

public static class Utils
{
    public static Vector3 NewVector3(float x, float y, float z)
    {
        return Vector3.right * x + Vector3.up * y + Vector3.forward * z;
    }

    public static Vector2 NewVector2(float x, float y)
    {
        return Vector2.right * x + Vector2.up * y;
    }
}