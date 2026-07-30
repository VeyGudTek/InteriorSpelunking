using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject Room;
    [SerializeField]
    private FreeSpaceManager FreeSpaceManager;
    [SerializeField]
    private Transform RoomParent;

    [Header("Settings")]
    [SerializeField]
    private int MaxAttempts = 5;
    [SerializeField]
    private float PlayingAreaLength = 100f;

    [Header("State")]
    [SerializeField]
    private int CurrentAttempts = 0;
    [SerializeField]
    private List<Room> Rooms = new();

    private void Start()
    {
        FreeSpaceManager.InitializeFreeSpace(Vector3.zero, Vector3.one * PlayingAreaLength);
    }

    private void Update()
    {
        if (FreeSpaceManager.TotalArea <= 25f)
        {
            return;
        }

        if (CurrentAttempts < MaxAttempts)
        {
            float availableAreaOffset = FreeSpaceManager.PercentageAvailable * 0.50f;
            float randomValue = Random.value;

            if (randomValue < 0.25f + availableAreaOffset || Rooms.Count == 0)
            {
                GenerateRandomRoom();
            }
            else
            {
                GenerateDuplicateRoom();
            }

            CurrentAttempts++;
        }
    }

    private void GenerateDuplicateRoom()
    {
        Room randomRoom = Rooms[Random.Range(0, Rooms.Count)];

        Vector3 randomOffset;
        int randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0:
                randomOffset = new(randomRoom.Length, 0f, 0f);
                break;
            case 1:
                randomOffset = new(-randomRoom.Length, 0f, 0f);
                break;
            case 2:
                randomOffset = new(0f, 0f, randomRoom.Width);
                break;
            case 3:
                randomOffset = new(0f, 0f, -randomRoom.Width);
                break;
            default:
                throw new System.ArgumentOutOfRangeException("Random Direction Surpassed 3.");
        }

        Vector3 newCenter = randomRoom.Center + randomOffset;
        int layerMask = LayerMask.GetMask(Layers.Obstacle);
        if (!Physics.CheckSphere(newCenter, Floats.CollisionPointRadius, layerMask))
        {
            GenerateRoom(newCenter, randomRoom.Size);
        }
    }

    private void GenerateRandomRoom()
    {
        Vector3 randomPoint = FreeSpaceManager.GetRandomPoint();
        Vector3 roomSize = new(Random.Range(5f, 15f), 1f, Random.Range(5f, 15f));

        GenerateRoom(randomPoint, roomSize);
    }

    private void GenerateRoom(Vector3 center, Vector3 size)
    {
        GameObject newRoomObject = Instantiate(Room, RoomParent);
        Room newRoom = newRoomObject.GetComponent<Room>();
        newRoom.Initialize(center, size);

        Rooms.Add(newRoom);

        FreeSpaceManager.UpdateSpaces();
    }
}
