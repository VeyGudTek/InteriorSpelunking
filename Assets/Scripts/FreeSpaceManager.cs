using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreeSpaceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject FreeSpacePrefab;

    private List<GameObject> FreeSpaces = new();

    public void InitializeFreeSpace(Vector3 center, Vector3 size)
    {
        FreeSpaces.Add(CreateFreeSpace(center, size));
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
            return new() { CreateFreeSpace(center, size) };
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
            newFreeSpaces.Add(CreateFreeSpace(center, size));
            clampedLeft = left;
        }
        if (right < originalRight)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(right, originalRight, originalForward, originalBack);
            newFreeSpaces.Add(CreateFreeSpace(center, size));
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

            newFreeSpaces.Add(CreateFreeSpace(center, size));
        }
        if (back > originalBack)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(newLeft, newRight, back, originalBack);

            newFreeSpaces.Add(CreateFreeSpace(center, size));
        }

        return newFreeSpaces;
    }

    private GameObject CreateFreeSpace(Vector3 center, Vector3 size)
    {
        GameObject newFreeSpace = Instantiate(FreeSpacePrefab, center, Quaternion.identity);
        newFreeSpace.transform.localScale = size;
        newFreeSpace.transform.SetParent(transform);

        Physics.SyncTransforms();

        return newFreeSpace;
    }

    public Vector3 GetRandomPoint()
    {
        float totalArea = FreeSpaces.Sum(fs => fs.transform.localScale.GetArea());
        float randomThreshold = Random.Range(0f, totalArea);

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
