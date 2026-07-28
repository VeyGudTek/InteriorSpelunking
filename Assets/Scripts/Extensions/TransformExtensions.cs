using UnityEngine;

public static class TransformExtensions
{
    public static (float left, float right, float forward, float back) GetBounds(this Transform transform)
    {
        return VectorExtensions.GetBounds(transform.position, transform.localScale);
    }
}
