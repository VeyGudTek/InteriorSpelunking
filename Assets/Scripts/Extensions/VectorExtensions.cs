using UnityEngine;

public static class VectorExtensions
{
    public static (Vector3 center, Vector3 size) ConvertToVector(float left, float right, float forward, float back)
    {
        Vector3 center = new((left + right) / 2f, 0f, (forward + back) / 2f);
        Vector3 size = new(right - left, 1f, forward - back);
        return (center, size);
    }
    public static (float left, float right, float forward, float back) GetBounds(Vector3 center, Vector3 size)
    {
        float leftBound = center.x - (size.x / 2f);
        float rightBound = center.x + (size.x / 2f);
        float forwardBound = center.z + (size.z / 2f);
        float backwardBound = center.z - (size.z / 2f);

        return (leftBound, rightBound, forwardBound, backwardBound);
    }

    public static float GetArea(this Vector3 size)
    {
        return size.x * size.z;
    }
}