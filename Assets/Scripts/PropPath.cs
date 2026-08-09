using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropPath : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject FakePropPrefab;

    const float PathExtrusion = 2f;

    public void GeneratePaths(List<Neighbor> neighbors)
    {

        foreach (Neighbor neighbor in neighbors.Where(n => n.HasPassage))
        {
            Vector3 pathSize = neighbor.SharedSide switch
            {
                Side.Left => new Vector3(PathExtrusion, PathExtrusion, neighbor.PassageWidth),
                Side.Right => new Vector3(PathExtrusion, PathExtrusion, neighbor.PassageWidth),
                Side.Forward => new Vector3(neighbor.PassageWidth, PathExtrusion, PathExtrusion),
                Side.Back => new Vector3(neighbor.PassageWidth, PathExtrusion, PathExtrusion),
                _ => throw new System.ArgumentOutOfRangeException($"Invalid shared side: {neighbor.SharedSide}")
            };

            if (CheckExisting(neighbor.PassagePoint))
            {
                continue;
            }

            GameObject newPath = Instantiate(FakePropPrefab, neighbor.PassagePoint, Quaternion.identity);
            newPath.transform.localScale = pathSize;
            newPath.transform.SetParent(transform);
        }

        Physics.SyncTransforms();
    }

    private bool CheckExisting(Vector3 passagePoint)
    {
        int layerMask = LayerMask.GetMask(Layers.Prop);
        Collider[] collisions = Physics.OverlapSphere(passagePoint, Floats.CollisionPointRadius, layerMask);
        foreach (Vector3 collisionCenter in collisions.Select(c => c.transform.position))
        {
            if ((collisionCenter - passagePoint).sqrMagnitude < Floats.CollisionPointRadius * Floats.CollisionPointRadius)
            {
                return true;
            }
        }

        return false;
    }
}
