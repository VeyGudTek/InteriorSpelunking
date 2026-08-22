using System.Collections.Generic;
using UnityEngine;

public class FloorAndCeilingGenerator : MonoBehaviour
{
    [Header("State")]
    public GenerationState State = GenerationState.Waiting;
    private int CurrentRoomIndex = 0;
    private List<Room> Rooms = new();

    public void StartFloorAndCeilingGeneration(List<Room> rooms)
    {
        Rooms = rooms;
        State = GenerationState.Generating;
    }

    private void Update()
    {
        if (State == GenerationState.Generating)
        {
            GenerateFloorsAndCeilings();
        }
    }

    private void GenerateFloorsAndCeilings()
    {
        if (CurrentRoomIndex < Rooms.Count)
        {
            Rooms[CurrentRoomIndex].GenerateFloorAndCeiling();
            CurrentRoomIndex++;
        }
        else
        {
            State = GenerationState.Completed;
        }
    }
}
