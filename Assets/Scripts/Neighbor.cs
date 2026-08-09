using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Neighbor
{
    //NeighborGenerator
    public Room OtherRoom;
    public Side SharedSide;
    public float SharedLength = 0f;

    //PathGenerator
    public bool HasPassage = false;

    //WallGenerator
    public Vector3 PassagePoint = Vector3.zero;
    public float PassageWidth = 0f;
}