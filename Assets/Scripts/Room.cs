using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private GameObject Obstacle;

    private float LeftBound = 0f;
    private float RightBound = 0f;
    private float ForwardBound = 0f;
    private float BackwardBound = 0f;

    private float CenterX => (LeftBound + RightBound) / 2f;
    private float CenterY => (ForwardBound + BackwardBound) / 2f;
    public Vector3 Center => new(CenterX, 0f, CenterY);
    public float Length => RightBound - LeftBound;
    public float Width => ForwardBound - BackwardBound;
    public Vector3 Size => new(Length, 1f, Width);

    public void Initialize(Vector3 center, Vector3 size)
    {
        (float left, float right, float forward, float back) = VectorExtensions.GetBounds(center, size);
        LeftBound = left;
        RightBound = right;
        ForwardBound = forward;
        BackwardBound = back;

        ClampBounds();
        UpdateObstacle();
    }

    private void ClampBounds()
    {
        int layerMask = LayerMask.GetMask(Layers.Obstacle);

        int breaker = 0;
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

            ClampSingleBound(collidedRooms.First());
        }  
    }

    private void ClampSingleBound(Room collidedRoom)
    {
        List<(Side side, float value)> possibleClamps = new();

        if (collidedRoom.LeftBound > CenterX)
        {
            possibleClamps.Add((Side.Right, collidedRoom.LeftBound));
        }
        if (collidedRoom.RightBound < CenterX)
        {
            possibleClamps.Add((Side.Left, collidedRoom.RightBound));
        }
        if (collidedRoom.ForwardBound < CenterY)
        {
            possibleClamps.Add((Side.Back, collidedRoom.ForwardBound));
        }
        if (collidedRoom.BackwardBound > CenterY)
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
        Obstacle.transform.position = new Vector3(CenterX, 0f, CenterY);
        Obstacle.transform.localScale = new Vector3(Length, 1f, Width);

        Physics.SyncTransforms();
    }
}
