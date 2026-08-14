using UnityEngine;

public static class SideExtensions
{
    public static Side GetRandomSide()
    {
        int randomInt = Random.Range(0, 4);
        return randomInt switch
        {
            0 => Side.Left,
            1 => Side.Right,
            2 => Side.Forward,
            3 => Side.Back,
            _ => throw new System.ArgumentOutOfRangeException(),
        };
    }

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
