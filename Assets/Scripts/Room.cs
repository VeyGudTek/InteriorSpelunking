using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private GameObject Obstacle;

    private float LeftBound = 0f;
    private float RightBound = 0f;
    private float ForwardBound = 0f;
    private float BackwardBound = 0f;

    private float CenterX => (LeftBound + RightBound) / 2f;
    private float CenterY => (ForwardBound + BackwardBound) / 2f;
    private float Length => RightBound - LeftBound;
    private float Width => ForwardBound - BackwardBound;

    public void Initialize(Vector3 center, Vector3 size)
    {
        (float left, float right, float forward, float back) = VectorExtensions.GetBounds(center, size);
        LeftBound = left;
        RightBound = right;
        ForwardBound = forward;
        BackwardBound = back;

        UpdateObstacle();
    }

    private void UpdateObstacle()
    {
        Obstacle.transform.position = new Vector3(CenterX, 0f, CenterY);
        Obstacle.transform.localScale = new Vector3(Length, 1f, Width);

        Physics.SyncTransforms();
    }
}
