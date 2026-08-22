using System;
using UnityEngine;

public class PropSettings : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject Collider;
    [SerializeField]
    private GameObject Visual;

    [Header("Settings")]
    public bool PathBlocking = false;
    public bool HasSupporting = false;

    [Header("State")]
    public Side? Orientation = null;

    public Vector3 GetSize()
    {
        return Collider.transform.localScale;
    }

    public void SetPosition(Quaternion rotation, Vector3 position, Side? orientation)
    {
        Orientation = orientation;

        Collider.transform.position = position;
        Visual.transform.position = position;

        Collider.transform.rotation = rotation;
        Visual.transform.rotation = rotation;
    }

    public void TryGenerateSupport(GameObject supportPrefab, float roomLeft, float roomRight, float roomForward, float roomBack, float level,
        Action<Collider[], PropSettings, GameObject, Vector3, Quaternion, Side?> tryInstantiatePropCallback
    )
    {
        PropSettings propData = supportPrefab.GetComponent<PropSettings>();

        (float horizontalOffset, float verticalOffset) = GetOffsets();
        Side randomSide = SideExtensions.GetRandomSide();

        Vector3 supportSize = propData.GetSize();
        Vector3 currentCenter = Collider.transform.position;
        float yOffset = level + (supportSize.y / 2f) - currentCenter.y;
        Vector3 centerPoint = randomSide switch
        {
            Side.Left =>    currentCenter + new Vector3(0f, yOffset, 0f) - new Vector3(horizontalOffset + supportSize.z / 2f, 0f, 0),
            Side.Right =>   currentCenter + new Vector3(0f, yOffset, 0f) + new Vector3(horizontalOffset + supportSize.z / 2f, 0f, 0),
            Side.Forward => currentCenter + new Vector3(0f, yOffset, 0f) + new Vector3(0, 0f, verticalOffset + supportSize.z / 2f),
            Side.Back =>    currentCenter + new Vector3(0f, yOffset, 0f) - new Vector3(0, 0f, verticalOffset + supportSize.z / 2f),
            _ => throw new System.Exception("Invalid side")
        };
        Quaternion rotation = randomSide.GetOpposite().GetRotation();

        if (PropExceedsRoom(randomSide, centerPoint, supportSize, roomLeft, roomRight, roomForward, roomBack))
        {
            return;
        }

        Vector3 halfExtents = supportSize / 2f - Vectors.OverlapThreshold;
        int layerMask = LayerMask.GetMask(Layers.Prop);
        Collider[] collisions = Physics.OverlapBox(centerPoint, halfExtents, rotation, layerMask);

        tryInstantiatePropCallback(collisions, propData, supportPrefab, centerPoint, rotation, randomSide.GetOpposite());
    }

    private (float horizontalOffset, float verticalOffset) GetOffsets()
    {
        Vector3 size = Collider.transform.localScale;

        if (!Orientation.HasValue)
        {
            return (size.x / 2f, size.z / 2f);
        }

        bool isHorizontal = Orientation.Value.IsHorizontal();
        float horizontalOffset = (isHorizontal ? size.z : size.x) / 2f;
        float verticalOffset = (isHorizontal ? size.x : size.z) / 2f;

        return (horizontalOffset, verticalOffset);
    }

    private bool PropExceedsRoom(Side propSide, Vector3 newPosition, Vector3 newSize, float roomLeft, float roomRight, float roomForward, float roomBack)
    {
        return propSide switch
        {
            Side.Left => newPosition.x - newSize.z / 2f < roomLeft,
            Side.Right => newPosition.x + newSize.z / 2f > roomRight,
            Side.Forward => newPosition.z + newSize.z / 2f > roomForward,
            Side.Back => newPosition.z - newSize.z / 2f < -roomBack,
            _ => throw new System.Exception("Invalid side")
        };
    }
}
