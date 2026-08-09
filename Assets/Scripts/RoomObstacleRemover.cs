using System.Collections.Generic;
using UnityEngine;

public class RoomObstacleRemover : MonoBehaviour
{
    public GenerationState State = GenerationState.Waiting;

    private int CurrentRoomIndex = 0;
    private List<Room> Rooms = new();

    public void StartObstacleRemoval(List<Room> rooms)
    {
        Rooms = rooms;
        State = GenerationState.Generating;
    }

    private void Update()
    {
        if (State == GenerationState.Generating)
        {
            RemoveObstacles();
        }
    }

    private void RemoveObstacles()
    {
        if (CurrentRoomIndex < Rooms.Count)
        {
            Room currentRoom = Rooms[CurrentRoomIndex];
            currentRoom.RemoveObstacle();

            CurrentRoomIndex++;
        }
        else
        {
            State = GenerationState.Completed;
        }
    }
}
