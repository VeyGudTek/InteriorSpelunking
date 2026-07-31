using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
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
    public GenerationState GenerationState = GenerationState.Waiting;
    [SerializeField]
    private int CurrentAttempts = 0;
    [SerializeField]
    private List<Room> Rooms = new();

    public void StartRoomGeneration()
    {
        FreeSpaceManager.InitializeFreeSpace(Vector3.zero, Vector3.one * PlayingAreaLength);
        GenerationState = GenerationState.Generating;
    }

    public List<Room> GetGeneratedRooms()
    {
        if (GenerationState != GenerationState.Completed)
        {
            throw new System.InvalidOperationException("Room generation is not completed yet.");
        }

        return Rooms.ToList();
    }

    private void Update()
    {
        if (GenerationState == GenerationState.Generating)
        {
            TryGenerateRoom();
        }
    }

    private void TryGenerateRoom()
    {
        if (CurrentAttempts < MaxAttempts && FreeSpaceManager.TotalArea > 25f)
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
        else
        {
            GenerationState = GenerationState.Completed;
            Destroy(FreeSpaceManager.gameObject);
            FreeSpaceManager = null;
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
