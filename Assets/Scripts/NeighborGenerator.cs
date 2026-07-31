using System.Collections.Generic;
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

    private void GenerateNeighborsForRoom(Room room)
    {
        int layerMask = LayerMask.GetMask(Layers.Obstacle);
        Vector3 halfExtent = (room.Size / 2f);

        Collider[] collisions = Physics.OverlapBox(room.Center, halfExtent, Quaternion.identity, layerMask);

        foreach (Collider collision in collisions)
        {
            Room collidedRoom = collision.GetComponentInParent<Room>();
            
        }
    }
}
