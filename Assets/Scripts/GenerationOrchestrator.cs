using System.Collections.Generic;
using UnityEngine;

public class GenerationOrchestrator : MonoBehaviour
{
    private enum OrchestrationState
    {
        RoomLayout,
        NeighborGeneration,
        PathGeneration,
        Finished
    }

    [Header("References")]
    [SerializeField]
    private RoomGenerator RoomGenerator;
    [SerializeField]
    private NeighborGenerator NeighborGenerator;
    [SerializeField]
    private PathGenerator PathGenerator;

    [Header("State")]
    [SerializeField]
    private OrchestrationState State = OrchestrationState.RoomLayout;
    [SerializeField]
    private List<Room> GeneratedRooms = new();

    private void Update()
    {
        switch (State)
        {
            case OrchestrationState.RoomLayout:
                ProcessRoomLayout();
                break;
            case OrchestrationState.NeighborGeneration:
                ProcessNeighborGeneration();
                break;
            case OrchestrationState.PathGeneration:
                ProcessPathGeneration();
                break;
            case OrchestrationState.Finished:
                //Done
                break;
            default:
                throw new System.InvalidOperationException("Undefined Orchestration State");
        }
    }

    private void ProcessRoomLayout()
    {
        if (RoomGenerator.GenerationState == GenerationState.Waiting)
        {
            RoomGenerator.StartRoomGeneration();
            return;
        }

        if (RoomGenerator.GenerationState == GenerationState.Completed)
        {
            GeneratedRooms = RoomGenerator.GetGeneratedRooms();
            State = OrchestrationState.NeighborGeneration;
        }
    }

    private void ProcessNeighborGeneration()
    {
        if (NeighborGenerator.State == GenerationState.Waiting)
        {
            NeighborGenerator.StartNeighborGeneration(GeneratedRooms);
            return;
        }
        if (NeighborGenerator.State == GenerationState.Completed)
        {
            State = OrchestrationState.PathGeneration;
        }
    }

    private void ProcessPathGeneration()
    {
        if (PathGenerator.State == GenerationState.Waiting)
        {
            PathGenerator.StartPathGeneration(GeneratedRooms);
            return;
        }
        if (PathGenerator.State == GenerationState.Completed)
        {
            State = OrchestrationState.Finished;
        }
    }
}
