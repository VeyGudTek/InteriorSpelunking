using UnityEngine;

public class FloorAndCeiling : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject Floor;
    [SerializeField]
    private GameObject Ceiling;

    [Header("Settings")]
    [SerializeField]
    private float FloorWidth = 0.5f;
    [SerializeField]
    private float CeilingWidth = 0.5f;

    public void GenerateFloorAndCeiling(float left, float right, float forward, float back, float level, float height)
    {
        (Vector3 floorPosition, Vector3 floorSize) = VectorExtensions.ConvertToVector(left, right, forward, back, level - FloorWidth / 2f, FloorWidth);
        (Vector3 ceilingPosition, Vector3 ceilingSize) = VectorExtensions.ConvertToVector(left, right, forward, back, level + height + CeilingWidth / 2f, CeilingWidth);

        GameObject floor = Instantiate(Floor, floorPosition, Quaternion.identity);
        floor.transform.localScale = floorSize;
        floor.transform.SetParent(transform);

        //COMMENTED FOR DEBUGGING PURPOSES
        //GameObject ceiling = Instantiate(Ceiling, ceilingPosition, Quaternion.identity);
        //ceiling.transform.localScale = ceilingSize;
        //ceiling.transform.SetParent(transform);
    }
}
