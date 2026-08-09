using System.Collections.Generic;
using UnityEngine;

public class PropPathGenerator : MonoBehaviour
{
    public GenerationState State = GenerationState.Waiting;

    [Header("State")]
    [SerializeField]
    private int CurrentRoomIndex = 0;
    [SerializeField]
    private List<Room> Rooms;

    public void StartPropPathGeneration(List<Room> rooms)
    {
        Rooms = rooms;

        State = GenerationState.Generating;
    }

    private void Update()
    {
        if (State == GenerationState.Generating)
        {
            GeneratePropPaths();
        }
    }

    private void GeneratePropPaths()
    {
        if (CurrentRoomIndex < Rooms.Count)
        {
            Room room = Rooms[CurrentRoomIndex];
            room.GeneratePropPaths();

            CurrentRoomIndex++;
        }
        else
        {
            State = GenerationState.Completed;
        }
    }
}
