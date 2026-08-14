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
        /* Implement Later
        if (GeneratedProps.Select(p => p.HasSupporting).Any() && Random.value > SupportingPropChance)
        {
            TryGenerateSupportingProp();
        }

        if (Random.value > RandomPropChance)
        {
            TryGenerateRandomProp();
        }

        if (Random.value > EdgePropChance)
        {
            TryGenerateEdgeProp();
        }
        */

        TryGenerateCenterProp();
    }

    private void TryGenerateSupportingProp()
    {
        //TODO
    }

    private void TryGenerateEdgeProp()
    {
        //TODO
    }

    private void TryGenerateRandomProp()
    {
        //TODO
    }

    private void TryGenerateCenterProp()
    {
        GameObject propPrefab = PropDatabase.GetRandomProp(PropType.Center);
        PropSettings propData = propPrefab.GetComponent<PropSettings>();

        Vector3 size = propData.GetSize();

        Side randomSide = SideExtensions.GetRandomSide();
        Quaternion randomRotation = randomSide.GetRotation();
        bool isHorizontalRotation = randomSide.IsHorizontal();
        float xClamp = isHorizontalRotation ? size.z / 2f : size.x / 2f;
        float zClamp = isHorizontalRotation ? size.x / 2f : size.z / 2f;

        if (xClamp * 2f > RightBound - LeftBound || zClamp * 2f > ForwardBound - BackBound)
        {
            return;
        }

        Vector3 randomPosition = new(
            Random.Range(LeftBound + xClamp, RightBound - xClamp),
            Level,
            Random.Range(BackBound + zClamp, ForwardBound - zClamp)
        );

        Vector3 halfExtents = size / 2f - Vectors.OverlapThreshold;
        int layerMask = LayerMask.GetMask(Layers.Prop);
        Collider[] collisions = Physics.OverlapBox(randomPosition, halfExtents, randomRotation, layerMask);

        if (collisions.Length == 0)
        {
            InstantiateProp(propPrefab, randomPosition, randomRotation);
            return;
        }

        if (!propData.PathBlocking && collisions.All(c => c.gameObject.CompareTag(Tags.Interior)))
        {
            InstantiateProp(propPrefab, randomPosition, randomRotation);
        }
    }

    private void InstantiateProp(GameObject prop, Vector3 position, Quaternion rotation)
    {
        GameObject newProp = Instantiate(prop, transform);
        PropSettings propSettings = newProp.GetComponent<PropSettings>();

        propSettings.SetPosition(rotation, position);
        GeneratedProps.Add(propSettings);
    }
}
