using System.Collections.Generic;
using UnityEngine;

public class PropPathInteriorGenerator : MonoBehaviour
{
    public GenerationState State = GenerationState.Waiting;

    [Header("State")]
    [SerializeField]
    private int CurrentRoomIndex = 0;
    [SerializeField]
    private List<Room> Rooms;

    public void StartInteriorPropPathGeneration(List<Room> rooms)
    {
        Rooms = rooms;

        State = GenerationState.Generating;
    }

    private void Update()
    {
        if (State == GenerationState.Generating)
        {
            GenerateInteriorPropPaths();
        }
    }

    private void GenerateInteriorPropPaths()
    {
        if (CurrentRoomIndex < Rooms.Count)
        {
            Room room = Rooms[CurrentRoomIndex];
            room.GenerateInteriorPropPaths();

            CurrentRoomIndex++;
        }
        else
        {
            State = GenerationState.Completed;
        }
    }
}
