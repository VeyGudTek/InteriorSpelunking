using System.Collections.Generic;
using UnityEngine;

public class Props : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject TempProp;
    [SerializeField]
    private PropPathGenerator PropPathGenerator;

    [Header("State")]
    public GenerationState State = GenerationState.Waiting;

    private float LeftBound;
    private float RightBound;
    private float ForwardBound;
    private float BackBound;
    private float Level;

    public void StartGeneration(float left, float right, float forward, float back, float level, List<Neighbor> neighbors)
    {
        AssignProperties(left, right, forward, back, level);
        PropPathGenerator.GeneratePath(left, right, forward, back, level, neighbors);

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

    private void Update()
    {
        if (State == GenerationState.Generating)
        {
            GenerateProps();
        }
    }

    private void GenerateProps()
    {
        
    }
}
