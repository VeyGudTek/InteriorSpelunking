public static class SideExtensions
{
    public static bool IsHorizontal(this Side side)
    {
        return side == Side.Left || side == Side.Right;
    }
}
