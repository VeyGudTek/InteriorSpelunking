using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropPath : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject FakePropPrefab;

    public void GeneratePaths(List<Neighbor> neighbors)
    {
        float pathRadius = Floats.MinimumDoorWidth * 2f;
        Vector3 pathSize = new(pathRadius, Floats.FreeSpaceHeight, pathRadius);

        foreach (Neighbor neighbor in neighbors.Where(n => n.HasPassage))
        {
            int layerMask = LayerMask.GetMask(Layers.Prop);
            Collider[] collisions = Physics.OverlapBox(neighbor.PassagePoint, pathSize / 2, Quaternion.identity, layerMask);
            if (collisions.Length > 0 )
            {
                continue;
            }

            GameObject newPath = Instantiate(FakePropPrefab, neighbor.PassagePoint, Quaternion.identity);
            newPath.transform.localScale = pathSize;
            newPath.transform.SetParent(transform);
        }

        Physics.SyncTransforms();
    }
}
