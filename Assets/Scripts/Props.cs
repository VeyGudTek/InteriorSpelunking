using System.Collections.Generic;
using UnityEngine;

public class Props : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject TempProp;

    [Header("State")]
    public GenerationState State = GenerationState.Waiting;
    private int CurrentAttempts = 0;
    private int MaxAttempts = 50;
    private List<GameObject> GeneratedProps = new();

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
            CurrentAttempts++;
        }
        else
        {
            State = GenerationState.Completed;
        }
    }
}
