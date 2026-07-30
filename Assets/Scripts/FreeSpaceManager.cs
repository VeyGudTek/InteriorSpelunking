using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreeSpaceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject FreeSpacePrefab;

    private List<GameObject> FreeSpaces = new();

    public float TotalArea => FreeSpaces.Sum(fs => fs.transform.localScale.GetArea());

    public void InitializeFreeSpace(Vector3 center, Vector3 size)
    {
        if (TryCreateFreeSpace(center, size, out GameObject freeSpace))
        {
            FreeSpaces.Add(freeSpace);
        }
        else
        {
            throw new System.InvalidOperationException("Initial free space is too small.");
        }
    }

    public void UpdateSpaces()
    {
        List<GameObject> newFreeSpaces = new();

        foreach (GameObject freeSpace in FreeSpaces)
        {
            newFreeSpaces.AddRange(GetNewFreeSpaces(freeSpace.transform));
            Destroy(freeSpace);
        }

        FreeSpaces = newFreeSpaces;
    }

    private List<GameObject> GetNewFreeSpaces(Transform spaceTransform)
    {
        Vector3 center = spaceTransform.position;
        Vector3 size = spaceTransform.localScale;
        Vector3 halfExtent = (size / 2f) - Vectors.OverlapThreshold;

        int layerMask = LayerMask.GetMask(Layers.Obstacle);

        Collider[] colliders = Physics.OverlapBox(center, halfExtent, Quaternion.identity, layerMask);

        if (colliders.Length == 0)
        {
            if (TryCreateFreeSpace(center, size, out GameObject freeSpace))
            {
                return new() { freeSpace };
            }
            else
            {
                throw new System.InvalidOperationException("Failed to create existing space.");
            }
        }

        if (colliders.Length > 1)
        {
            throw new System.InvalidOperationException("Multiple colliders detected. Update free spaces once per new obstacle.");
        }

        return SplitSpace(colliders[0].transform, spaceTransform);
    }

    private List<GameObject> SplitSpace(Transform collision, Transform original)
    {
        (float originalLeft, float originalRight, float originalForward, float originalBack) = original.GetBounds();
        (float left, float right, float _, float _) = collision.GetBounds();

        List<GameObject> newFreeSpaces = new();

        float clampedLeft = originalLeft;
        float clampedRight = originalRight;
        if (left > originalLeft)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(originalLeft, left, originalForward, originalBack);
            if (TryCreateFreeSpace(center, size, out GameObject freeSpace))
            {
                newFreeSpaces.Add(freeSpace);
            }
            clampedLeft = left;
        }
        if (right < originalRight)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(right, originalRight, originalForward, originalBack);
            if (TryCreateFreeSpace(center, size, out GameObject freeSpace))
            {
                newFreeSpaces.Add(freeSpace);
            }
            clampedRight = right;
        }

        newFreeSpaces.AddRange(SplitSpaceVertical(collision, original, clampedLeft, clampedRight));

        return newFreeSpaces;
    }

    private List<GameObject> SplitSpaceVertical(Transform collision, Transform original, float newLeft, float newRight)
    {
        (float _, float _, float originalForward, float originalBack) = original.GetBounds();
        (float _, float _, float forward, float back) = collision.GetBounds();

        List<GameObject> newFreeSpaces = new();

        if (forward < originalForward)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(newLeft, newRight, originalForward, forward);

            if (TryCreateFreeSpace(center, size, out GameObject freeSpace))
            {
                newFreeSpaces.Add(freeSpace);
            }
        }
        if (back > originalBack)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(newLeft, newRight, back, originalBack);

            if (TryCreateFreeSpace(center, size, out GameObject freeSpace))
            {
                newFreeSpaces.Add(freeSpace);
            }
        }

        return newFreeSpaces;
    }

    private bool TryCreateFreeSpace(Vector3 center, Vector3 size, out GameObject freeSpace)
    {
        freeSpace = null;

        if (size.GetArea() < Floats.MinimumFreeSpaceArea)
        {
            return false;
        }

        freeSpace = Instantiate(FreeSpacePrefab, center, Quaternion.identity);
        freeSpace.transform.localScale = size;
        freeSpace.transform.SetParent(transform);

        Physics.SyncTransforms();

        return true;
    }

    public Vector3 GetRandomPoint()
    {
        float randomThreshold = Random.Range(0f, TotalArea);
        float cumulativeArea = 0f;

        foreach(GameObject freeSpace in FreeSpaces)
        {
            float area = freeSpace.transform.localScale.GetArea();
            cumulativeArea += area;
            if (cumulativeArea >= randomThreshold)
            {
                Vector3 center = freeSpace.transform.position;
                Vector3 size = freeSpace.transform.localScale;

                float randomX = Random.Range(center.x - (size.x / 2f), center.x + (size.x / 2f));
                float randomZ = Random.Range(center.z - (size.z / 2f), center.z + (size.z / 2f));

                return new Vector3(randomX, 0f, randomZ);
            }
        }

        throw new System.InvalidOperationException("Cumulative area surpasses total area.");
    }
}
