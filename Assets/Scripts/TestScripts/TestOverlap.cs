using UnityEngine;

public class TestOverlap : MonoBehaviour
{
    public Transform Transform1;
    public Transform Transform2;

    void Update()
    {
        CheckOverlap();
    }

    private void CheckOverlap()
    {
        Vector3 point1 = Transform1.position;
        Vector3 point2 = Transform2.position;

        Vector3 midpoint = (point1 + point2) / 2f;
        float length = Vector3.Distance(point1, point2);
        Vector3 halfExtent = new Vector3(.5f, .5f, length / 2f);

        Vector3 direction = (point2 - point1).normalized;

        Quaternion orientation = Quaternion.LookRotation(direction);

        Collider[] colliders = Physics.OverlapBox(midpoint, halfExtent, orientation);

        if (colliders.Length > 0)
        {
            Debug.Log("Overlapped");
        }
    }
}
