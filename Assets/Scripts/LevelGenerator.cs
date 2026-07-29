using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject Room;
    [SerializeField]
    private FreeSpaceManager FreeSpaceManager;

    [Header("Settings")]
    [SerializeField]
    private int TotalRooms = 5;
    [SerializeField]
    private float PlayingAreaLength = 100f;

    private List<Room> Rooms = new();

    private void Start()
    {
        FreeSpaceManager.InitializeFreeSpace(Vector3.zero, Vector3.one * PlayingAreaLength);
    }

    private void Update()
    {
        if (Rooms.Count < TotalRooms)
        {
            GenerateRoom();
        }
    }

    private void GenerateRoom()
    {
        Vector3 randomPoint = FreeSpaceManager.GetRandomPoint();

        Vector3 roomSize = new(Random.Range(5f, 15f), 1f, Random.Range(5f, 15f));

        GameObject newRoomObject = Instantiate(Room);
        Room newRoom = newRoomObject.GetComponent<Room>();
        newRoom.Initialize(randomPoint, roomSize);

        Rooms.Add(newRoom);

        FreeSpaceManager.UpdateSpaces();
    }
}
