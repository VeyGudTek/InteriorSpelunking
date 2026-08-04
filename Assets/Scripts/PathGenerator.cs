using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [Header("State")]
    public GenerationState State = GenerationState.Waiting;
    private List<Room> AllRooms = new();
    private readonly List<Room> ActiveRooms = new();

    public void StartPathGeneration(List<Room> allRooms)
    {
        AllRooms = allRooms;

        int randomIndex = Random.Range(0, AllRooms.Count);
        AddActiveRoom(AllRooms[randomIndex]);

        State = GenerationState.Generating;
    }

    void Update()
    {
        if (State == GenerationState.Generating)
        {
            CreatePath();
        }
    }

    private void CreatePath()
    {
        if (ActiveRooms.Count == 0)
        {
            State = GenerationState.Completed;
            return;
        }

        int randomIndex = Random.Range(0, ActiveRooms.Count);
        Room randomRoom = ActiveRooms[randomIndex];

        List<Room> unvisitedNeighbors = randomRoom.Neighbors.Where(n => !n.OtherRoom.Visited && n.SharedLength > Floats.MinimumDoorWidth).Select(n => n.OtherRoom).ToList();
        if (unvisitedNeighbors.Count == 0)
        {
            ActiveRooms.Remove(randomRoom);
            return;
        }

        int randomNeighborIndex = Random.Range(0, unvisitedNeighbors.Count);
        Room randomNeighbor = unvisitedNeighbors[randomNeighborIndex];

        AddActiveRoom(randomNeighbor);
        OpenPath(randomRoom, randomNeighbor);
    }

    private void AddActiveRoom(Room room)
    {
        ActiveRooms.Add(room);
        room.Visited = true;
    }

    private void OpenPath(Room neighborOne,  Room neighborTwo)
    {
        neighborOne.Neighbors.Where(n => n.OtherRoom == neighborTwo).First().HasPassage = true;
        neighborTwo.Neighbors.Where(n => n.OtherRoom == neighborOne).First().HasPassage = true;
    }
}
