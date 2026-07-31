using System;

[Serializable]
public class Neighbor
{
    Room OtherRoom;
    Side SharedSide;
    bool HasPassage;
}