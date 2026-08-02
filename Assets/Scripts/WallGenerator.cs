using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [Header("State")]
    public GenerationState State = GenerationState.Waiting;
    private int CurrentRoomIndex = 0;
    private List<Room> Rooms = new();

    public void StartWallGeneration(List<Room> rooms)
    {
        Rooms = rooms;
        State = GenerationState.Generating;
    }

    private void Update()
    {
        if (State == GenerationState.Generating)
        {
            GenerateWalls();
        }
    }

    private void GenerateWalls()
    {
        if (CurrentRoomIndex < Rooms.Count)
        {
            Rooms[CurrentRoomIndex].GenerateWalls();
            CurrentRoomIndex++;
        }
        else
        {
            State = GenerationState.Completed;
        }
    }
}
