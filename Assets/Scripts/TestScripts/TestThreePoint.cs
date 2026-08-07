using UnityEngine;

public class TestThreePoint : MonoBehaviour
{
    public GameObject SpawningPlane;
    public GameObject PointPrefab;
    public int NumPoints;

    private void Start()
    {
        Vector3 range = SpawningPlane.transform.localScale / 2f;
        Vector3 startingPoint = SpawningPlane.transform.localPosition;

        for (int i = 0; i < NumPoints; i++)
        {
            Vector3 spawnPoint = startingPoint + new Vector3(
                Random.Range(-range.x, range.x),
                2f,
                Random.Range(-range.z, range.z)
            );

            Instantiate(PointPrefab, spawnPoint, Quaternion.identity, transform);
        }
    }
}
