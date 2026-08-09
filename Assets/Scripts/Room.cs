using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject Obstacle;
    public Walls Walls;
    public PropPath PropPath;
    public Props Props;

    [Header("Dimensions")]
    public float LeftBound = 0f;
    public float RightBound = 0f;
    public float ForwardBound = 0f;
    public float BackwardBound = 0f;
    public float Height = 0f;
    public float Level = 0f;

    [Header("Settings")]
    public bool IsStarting = false;

    [Header("Neighbor Information")]
    public bool Visited = false;
    public List<Neighbor> Neighbors = new();

    private float CenterX => (LeftBound + RightBound) / 2f;
    private float CenterZ => (ForwardBound + BackwardBound) / 2f;
    public Vector3 Center => new(CenterX, Level, CenterZ);
    public float Length => RightBound - LeftBound;
    public float Width => ForwardBound - BackwardBound;
    public Vector3 Size => new(Length, Height, Width);

    public void Initialize(Vector3 center, Vector3 size, float height, float level)
    {
        (float left, float right, float forward, float back) = VectorExtensions.GetBounds(center, size);
        LeftBound = left;
        RightBound = right;
        ForwardBound = forward;
        BackwardBound = back;
        Height = height;
        Level = level;

        ClampBounds();
        UpdateObstacle();
    }

    private void ClampBounds()
    {
        int layerMask = LayerMask.GetMask(Layers.Obstacle);

        int breaker = 0;
        Vector3 originalCenter = Center;
        while (true)
        {
            breaker++;
            if (breaker > 1000)
            {
                throw new System.InvalidOperationException("Clamping Loop surpassed 1000 iterations.");
            }

            Vector3 halfExtent = (Size / 2f) - Vectors.OverlapThreshold;
            Collider[] colliders = Physics.OverlapBox(Center, halfExtent, Quaternion.identity, layerMask);
            IEnumerable<Room> collidedRooms = colliders.Select(c => c.GetComponentInParent<Room>()).Where(r => r != this);

            if (collidedRooms.Count() == 0)
            {
                break;
            }

            ClampSingleBound(collidedRooms.First(), originalCenter);
        }  
    }

    private void ClampSingleBound(Room collidedRoom, Vector3 originalCenter)
    {
        List<(Side side, float value)> possibleClamps = new();

        if (collidedRoom.LeftBound > originalCenter.x)
        {
            possibleClamps.Add((Side.Right, collidedRoom.LeftBound));
        }
        if (collidedRoom.RightBound < originalCenter.x)
        {
            possibleClamps.Add((Side.Left, collidedRoom.RightBound));
        }
        if (collidedRoom.ForwardBound < originalCenter.z)
        {
            possibleClamps.Add((Side.Back, collidedRoom.ForwardBound));
        }
        if (collidedRoom.BackwardBound > originalCenter.z)
        {
            possibleClamps.Add((Side.Forward, collidedRoom.BackwardBound));
        }

        if (possibleClamps.Count == 0 || possibleClamps.Count > 2)
        {
            throw new System.InvalidOperationException($"Unexpected number of clamps: {possibleClamps.Count}");
        }

        int clampIndex = Random.Range(0, possibleClamps.Count);
        (Side clampSide, float clampValue) = possibleClamps[clampIndex];

        if (clampSide == Side.Left)
        {
            LeftBound = clampValue;
        }
        else if (clampSide == Side.Right)
        {
            RightBound = clampValue;
        }
        else if (clampSide == Side.Forward)
        {
            ForwardBound = clampValue;
        }
        else if (clampSide == Side.Back)
        {
            BackwardBound = clampValue;
        }
    }

    private void UpdateObstacle()
    {
        Obstacle.transform.position = new Vector3(CenterX, Level, CenterZ);
        Obstacle.transform.localScale = new Vector3(Length, Floats.FreeSpaceHeight, Width);

        Physics.SyncTransforms();
    }

    public void GenerateWalls()
    {
        Walls.CreateWalls(LeftBound, RightBound, ForwardBound, BackwardBound, Height, Level, this, Neighbors);
    }

    public void GeneratePropPaths()
    {
        PropPath.GeneratePaths(Neighbors);
    }

    public void StartPropGeneration()
    {
        Props.StartGeneration(LeftBound, RightBound, ForwardBound, BackwardBound, Level);
    }
}
