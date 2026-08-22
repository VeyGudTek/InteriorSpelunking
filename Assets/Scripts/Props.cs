using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Props : MonoBehaviour
{
    const float EdgePropChance = 0.5f;
    const float SupportingPropChance = 0.33f;
    const float RandomPropChance = 0.1f;

    [Header("References")]
    [SerializeField]
    private PropDatabase PropDatabase;

    [Header("State")]
    public GenerationState State = GenerationState.Waiting;
    private int CurrentAttempts = 0;
    private int MaxAttempts = 50;

    private List<PropSettings> GeneratedProps = new();

    private float LeftBound;
    private float RightBound;
    private float ForwardBound;
    private float BackBound;
    private float Level;

    public void StartGeneration(float left, float right, float forward, float back, float level)
    {
        AssignProperties(left, right, forward, back, level);

        State = GenerationState.Generating;
    }

    private void AssignProperties(float left, float right, float forward, float back, float level)
    {
        LeftBound = left;
        RightBound = right;
        ForwardBound = forward;
        BackBound = back;
        Level = level;
    }

    public void GenerateProps()
    {
        if (CurrentAttempts < MaxAttempts)
        {
            TryGenerateProp();
            CurrentAttempts++;
        }
        else
        {
            State = GenerationState.Completed;
        }
    }

    private void TryGenerateProp()
    {
        List<PropSettings> propsWithSupports = GeneratedProps.Where(p => p.HasSupporting).ToList();
        if (propsWithSupports.Count > 0 && Random.value > SupportingPropChance)
        {
            GameObject supportProp = PropDatabase.GetRandomProp(PropType.Supporting);
            int randomIndex = Random.Range(0, propsWithSupports.Count);
            
            propsWithSupports[randomIndex].TryGenerateSupport(supportProp, LeftBound, RightBound, ForwardBound, BackBound, Level, TryInstantiateProp);
        }

        if (Random.value < RandomPropChance)
        {
            TryGenerateRandomProp();
        }

        if (Random.value < EdgePropChance)
        {
            TryGenerateEdgeProp();
        }

        TryGenerateCenterProp();
    }

    private void TryGenerateRandomProp()
    {
        GameObject propPrefab = PropDatabase.GetRandomProp(PropType.Random);
        PropSettings propData = propPrefab.GetComponent<PropSettings>();

        Vector3 size = propData.GetSize();
        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0, 360f), 0f);

        if (PropExceedsRoom(true, size))
        {
            return;
        }

        float clamp = (size.x / 2f) + (Floats.WallThickness / 2f);
        Vector3 randomPosition = new(
            Random.Range(LeftBound + clamp, RightBound - clamp),
            Level + size.y / 2f,
            Random.Range(BackBound + clamp, ForwardBound - clamp)
        );

        float halfExtent = size.x / 2f - Floats.OverlapThreshold;
        int layerMask = LayerMask.GetMask(Layers.Prop);
        Collider[] collisions = Physics.OverlapSphere(randomPosition, halfExtent, layerMask);

        TryInstantiateProp(collisions, propData, propPrefab, randomPosition, randomRotation, null);
    }

    private void TryGenerateEdgeProp()
    {
        GameObject propPrefab = PropDatabase.GetRandomProp(PropType.Edge);
        PropSettings propData = propPrefab.GetComponent<PropSettings>();

        Vector3 propSize = propData.GetSize();
        Side randomRoomSide = SideExtensions.GetRandomSide();

        if (PropExceedsRoom(randomRoomSide.IsHorizontal(), propSize))
        {
            return;
        }

        float roomBoundValue = randomRoomSide switch
        {
            Side.Left => LeftBound,
            Side.Right => RightBound,
            Side.Forward => ForwardBound,
            Side.Back => BackBound,
            _ => throw new System.ArgumentOutOfRangeException()
        };
        
        float wallOffset = randomRoomSide.IsPositive() ? -(propSize.z / 2f) - (Floats.WallThickness / 2f) : (propSize.z / 2f) + (Floats.WallThickness / 2f);
        float horizontalRoomClamp = (propSize.x / 2f) + (Floats.WallThickness / 2f);
        Vector3 newPosition = new Vector3(
            randomRoomSide.IsHorizontal() ? roomBoundValue + wallOffset : Random.Range(LeftBound + horizontalRoomClamp, RightBound - horizontalRoomClamp),
            Level + propSize.y / 2f,
            randomRoomSide.IsHorizontal() ? Random.Range(BackBound + horizontalRoomClamp, ForwardBound - horizontalRoomClamp) : roomBoundValue + wallOffset
        );

        Quaternion newRotation = randomRoomSide.GetOpposite().GetRotation();
        Vector3 halfExtents = propSize / 2f - Vectors.OverlapThreshold;
        int layerMask = LayerMask.GetMask(Layers.Prop);
        Collider[] collisions = Physics.OverlapBox(newPosition, halfExtents, newRotation, layerMask);

        TryInstantiateProp(collisions, propData, propPrefab, newPosition, newRotation, randomRoomSide.GetOpposite());
    }

    private void TryGenerateCenterProp()
    {
        GameObject propPrefab = PropDatabase.GetRandomProp(PropType.Center);
        PropSettings propData = propPrefab.GetComponent<PropSettings>();

        Vector3 size = propData.GetSize();

        Side randomSide = SideExtensions.GetRandomSide();
        Quaternion randomRotation = randomSide.GetRotation();
        bool isHorizontalRotation = randomSide.IsHorizontal();

        if (PropExceedsRoom(isHorizontalRotation, size))
        {
            return;
        }

        float xClamp = isHorizontalRotation ? size.z / 2f : size.x / 2f;
        float zClamp = isHorizontalRotation ? size.x / 2f : size.z / 2f;
        xClamp += Floats.WallThickness / 2f;
        zClamp += Floats.WallThickness / 2f;
        Vector3 randomPosition = new(
            Random.Range(LeftBound + xClamp, RightBound - xClamp),
            Level + size.y / 2f,
            Random.Range(BackBound + zClamp, ForwardBound - zClamp)
        );

        Vector3 halfExtents = size / 2f - Vectors.OverlapThreshold;
        int layerMask = LayerMask.GetMask(Layers.Prop);
        Collider[] collisions = Physics.OverlapBox(randomPosition, halfExtents, randomRotation, layerMask);

        TryInstantiateProp(collisions, propData, propPrefab, randomPosition, randomRotation, randomSide);
    }

    private bool PropExceedsRoom(bool isRotated, Vector3 propSize)
    {
        float xSize = isRotated ? propSize.z : propSize.x;
        float zSize = isRotated ? propSize.x : propSize.z;

        return xSize > RightBound - LeftBound - Floats.WallThickness || zSize > ForwardBound - BackBound - Floats.WallThickness;
    }

    private void TryInstantiateProp(Collider[] collisions, PropSettings propSettings, GameObject prop, Vector3 position, Quaternion rotation, Side? orientation)
    {
        if (propSettings.PathBlocking && collisions.Any(c => c.gameObject.CompareTag(Tags.Interior)))
        {
            return;
        }

        if (collisions.Any(c => !c.gameObject.CompareTag(Tags.Interior)))
        {
            return;
        }

        GameObject newProp = Instantiate(prop, transform);
        PropSettings newPropSettings = newProp.GetComponent<PropSettings>();

        newPropSettings.SetPosition(rotation, position, orientation);
        GeneratedProps.Add(newPropSettings);
    }
}
