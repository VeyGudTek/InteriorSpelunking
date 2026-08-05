using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Neighbor
{
    //Initialized Variables
    public Room OtherRoom;
    public Side SharedSide;
    public float SharedLength = 0f;

    //Hydrated Variables
    public bool HasPassage = false;
    public Vector3 PassagePoint = Vector3.zero;
}