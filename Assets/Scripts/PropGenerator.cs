using System.Collections.Generic;
using UnityEngine;

public class PropGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private int BatchSize = 50;

    [Header("State")]
    public GenerationState State = GenerationState.Waiting;
    private List<Room> Rooms;
    private int CurrentBatchIndex = 0;

    private int StartRoomIndex => CurrentBatchIndex * BatchSize;
    private int EndRoomIndex => Mathf.Min(StartRoomIndex + BatchSize, Rooms.Count);

    public void StartPropGeneration(List<Room> rooms)
    {
        Rooms = rooms;

        State = GenerationState.Generating;
        StartNewBatchGeneration();
    }

    private void Update()
    {
        if (State == GenerationState.Generating)
        {
            if (TryIncrementBatch())
            {
                StartNewBatchGeneration();
            }
            else
            {
                BatchGenerateProps();
            }
        }

    }

    private bool TryIncrementBatch()
    {
        for (int i = StartRoomIndex; i < EndRoomIndex; i++)
        {
            Room room = Rooms[i];
            if (room.Props.State == GenerationState.Generating)
            {
                return false;
            }
        }

        CurrentBatchIndex++;
        if (StartRoomIndex >= Rooms.Count)
        {
            State = GenerationState.Completed;
            return false;
        }

        return true;
    }

    private void StartNewBatchGeneration()
    {
        for (int i = StartRoomIndex; i < EndRoomIndex; i++)
        {
            Room room = Rooms[i];
            room.StartPropGeneration();
        }
    }

    private void BatchGenerateProps()
    {
        for (int i = StartRoomIndex; i < EndRoomIndex; i++)
        {
            Room room = Rooms[i];
            if (room.Props.State == GenerationState.Generating)
            {
                room.Props.GenerateProps();
            }
        }
        Physics.SyncTransforms();
    }
}
