using System;

[Serializable]
public class Neighbor
{
    public Room OtherRoom;
    public Side SharedSide;
    public bool HasPassage;
    public float SharedLength;
}