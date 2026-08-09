using UnityEngine;

public static class SideExtensions
{
    public static bool IsHorizontal(this Side side)
    {
        return side == Side.Left || side == Side.Right;
    }

    public static Quaternion GetRotation(this Side side)
    {
        return side switch
        {
            Side.Left => Quaternion.Euler(0, -90, 0),
            Side.Right => Quaternion.Euler(0, 90, 0),
            Side.Forward => Quaternion.Euler(0, 0, 0),
            Side.Back => Quaternion.Euler(0, 180, 0),
            _ => Quaternion.identity,
        };
    }
}
