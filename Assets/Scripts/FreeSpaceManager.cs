using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreeSpaceManager : MonoBehaviour
{
    [System.Serializable]
    private class FreeSpace
    {
        public float Left;
        public float Right;
        public float Forward;
        public float Back;

        public Vector3 Position;
        public Vector3 Size;
    }

    //If Debug is needed, use Gizmos.DrawWireCube
    [SerializeField]
    private List<FreeSpace> FreeSpaces = new();

    public float TotalArea => FreeSpaces.Sum(fs => fs.Size.GetArea());
    private float InitialArea = 0f;
    public float PercentageAvailable => TotalArea / InitialArea;
    private float Level = 0f;

    public void InitializeFreeSpace(Vector3 center, Vector3 size, float level)
    {
        Level = level;
        if (TryCreateFreeSpace(center, size, out FreeSpace freeSpace))
        {
            FreeSpaces.Add(freeSpace);
            InitialArea = TotalArea;
        }
        else
        {
            throw new System.InvalidOperationException("Initial free space is too small.");
        }
    }

    public void UpdateSpaces()
    {
        List<FreeSpace> newFreeSpaces = new();

        foreach (FreeSpace freeSpace in FreeSpaces)
        {
            newFreeSpaces.AddRange(GetNewFreeSpaces(freeSpace));
        }

        FreeSpaces = newFreeSpaces;
    }

    private List<FreeSpace> GetNewFreeSpaces(FreeSpace space)
    {
        Vector3 center = space.Position;
        Vector3 size = space.Size;
        Vector3 halfExtent = (size / 2f) - Vectors.OverlapThreshold;

        int layerMask = LayerMask.GetMask(Layers.Obstacle);

        Collider[] colliders = Physics.OverlapBox(center, halfExtent, Quaternion.identity, layerMask);

        if (colliders.Length == 0)
        {
            if (TryCreateFreeSpace(center, size, out FreeSpace freeSpace))
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

        return SplitSpace(colliders[0].transform, space);
    }

    private List<FreeSpace> SplitSpace(Transform collision, FreeSpace original)
    {
        float originalLeft = original.Left;
        float originalRight = original.Right;
        float originalForward = original.Forward;
        float originalBack = original.Back;
        (float left, float right, float _, float _) = collision.GetBounds();

        List<FreeSpace> newFreeSpaces = new();

        float clampedLeft = originalLeft;
        float clampedRight = originalRight;
        if (left > originalLeft)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(originalLeft, left, originalForward, originalBack, Level);
            if (TryCreateFreeSpace(center, size, out FreeSpace freeSpace))
            {
                newFreeSpaces.Add(freeSpace);
            }
            clampedLeft = left;
        }
        if (right < originalRight)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(right, originalRight, originalForward, originalBack, Level);
            if (TryCreateFreeSpace(center, size, out FreeSpace freeSpace))
            {
                newFreeSpaces.Add(freeSpace);
            }
            clampedRight = right;
        }

        newFreeSpaces.AddRange(SplitSpaceVertical(collision, original, clampedLeft, clampedRight));

        return newFreeSpaces;
    }

    private List<FreeSpace> SplitSpaceVertical(Transform collision, FreeSpace original, float newLeft, float newRight)
    {
        float originalForward = original.Forward;
        float originalBack = original.Back;
        (float _, float _, float forward, float back) = collision.GetBounds();

        List<FreeSpace> newFreeSpaces = new();

        if (forward < originalForward)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(newLeft, newRight, originalForward, forward, Level);

            if (TryCreateFreeSpace(center, size, out FreeSpace freeSpace))
            {
                newFreeSpaces.Add(freeSpace);
            }
        }
        if (back > originalBack)
        {
            (Vector3 center, Vector3 size) = VectorExtensions.ConvertToVector(newLeft, newRight, back, originalBack, Level);

            if (TryCreateFreeSpace(center, size, out FreeSpace freeSpace))
            {
                newFreeSpaces.Add(freeSpace);
            }
        }

        return newFreeSpaces;
    }

    private bool TryCreateFreeSpace(Vector3 center, Vector3 size, out FreeSpace freeSpace)
    {
        freeSpace = null;

        if (size.GetArea() < Floats.MinimumFreeSpaceArea)
        {
            return false;
        }

        (float left, float right, float forward, float back) = VectorExtensions.GetBounds(center, size);
        freeSpace = new FreeSpace()
        {
            Left = left,
            Right = right,
            Forward = forward,
            Back = back,

            Position = center,
            Size = size
        };

        return true;
    }

    public Vector3 GetRandomPoint()
    {
        float randomThreshold = Random.Range(0f, TotalArea);
        float cumulativeArea = 0f;

        foreach(FreeSpace freeSpace in FreeSpaces)
        {
            float area = freeSpace.Size.GetArea();
            cumulativeArea += area;
            if (cumulativeArea >= randomThreshold)
            {
                float randomX = Random.Range(freeSpace.Left, freeSpace.Right);
                float randomZ = Random.Range(freeSpace.Back, freeSpace.Forward);

                return new Vector3(randomX, Level, randomZ);
            }
        }

        throw new System.InvalidOperationException("Cumulative area surpasses total area.");
    }
}
