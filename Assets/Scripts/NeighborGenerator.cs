using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeighborGenerator : MonoBehaviour
{
    [Header("State")]
    public GenerationState State = GenerationState.Waiting;
    [SerializeField]
    private List<Room> Rooms = new();
    [SerializeField]
    private int currentRoomIndex = 0;

    public void StartNeighborGeneration(List<Room> rooms)
    {
        Rooms = rooms;
        State = GenerationState.Generating;
    }

    private void Update()
    {
        if (State == GenerationState.Generating)
        {
            TryGenerateNeighbor();
        }
    }

    private void TryGenerateNeighbor()
    {
        if (currentRoomIndex < Rooms.Count)
        {
            Room currentRoom = Rooms[currentRoomIndex];
            GenerateNeighborsForRoom(currentRoom);
            currentRoomIndex++;
        }
        else
        {
            State = GenerationState.Completed;
        }
    }

    private void GenerateNeighborsForRoom(Room currentRoom)
    {
        int layerMask = LayerMask.GetMask(Layers.Obstacle);
        Vector3 halfExtent = (currentRoom.Size / 2f);

        Collider[] collisions = Physics.OverlapBox(currentRoom.Center, halfExtent, Quaternion.identity, layerMask);
        IEnumerable<Room> collidedRooms = collisions.Select(c => c.GetComponentInParent<Room>()).Where(r => r != currentRoom);

        foreach (Room collidedRoom in collidedRooms)
        {
            if (TryGetSharedSide(currentRoom, collidedRoom, out Side sharedSide))
            {
                currentRoom.AddNeighbor(new()
                {
                    OtherRoom = collidedRoom,
                    SharedSide = sharedSide,
                    HasPassage = true
                });
            }
        }
    }

    private bool TryGetSharedSide(Room currentRoom, Room collidedRoom, out Side sharedSide)
    {
        sharedSide = Side.Left;
        int sharedCount = 0;


        if (Mathf.Abs(currentRoom.LeftBound - collidedRoom.RightBound) < Floats.EqualityThreshold)
        {
            sharedSide = Side.Left;
            sharedCount++;
        }
        if (Mathf.Abs(currentRoom.RightBound - collidedRoom.LeftBound) < Floats.EqualityThreshold)
        {
            sharedSide = Side.Right;
            sharedCount++;
        }
        if (Mathf.Abs(currentRoom.ForwardBound - collidedRoom.BackwardBound) < Floats.EqualityThreshold)
        {
            sharedSide = Side.Forward;
            sharedCount++;
        }
        if (Mathf.Abs(currentRoom.BackwardBound - collidedRoom.ForwardBound) < Floats.EqualityThreshold)
        {
            sharedSide = Side.Back;
            sharedCount++;
        }

        if (sharedCount == 0 || sharedCount > 2)
        {
            throw new System.InvalidOperationException($"Unexpected number of shared edges: {sharedCount}");
        }

        if (sharedCount == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
