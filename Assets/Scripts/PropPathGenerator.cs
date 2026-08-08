using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Generates Path between entry points. For now, do not use and see what kind of rooms are created without paths.
/// </summary>
public class PropPathGenerator : MonoBehaviour
{
    private const float EntryOffset = 0.25f;
    private const float PathWidth = 0.25f;

    [System.Serializable]
    private class PathNode
    {
        public Vector3 Point;
        public List<PathNode> Neighbors = new();
    }

    [Header("References")]
    [SerializeField]
    private GameObject PathPrefab;

    [Header("State")]
    [SerializeField]
    private List<PathNode> Path;

    private float LeftBound;
    private float RightBound;
    private float ForwardBound;
    private float BackBound;
    private float Level;

    public void GeneratePath(float left, float right, float forward, float back, float level, List<Neighbor> neighbors)
    {
        LeftBound = left;
        RightBound = right;
        ForwardBound = forward;
        BackBound = back;
        Level = level;

        List<PathNode> entryNodes = GetEntryNodes(neighbors);
        List<PathNode> interiorNode = GetRandomInteriorNodes();

        CreateMinimumSpanningTree(interiorNode);
        ConnectEntryAndInterior(entryNodes, interiorNode);

        List<PathNode> allNodes = entryNodes.Concat(interiorNode).ToList();
        InstantiatePaths(allNodes);
        Physics.SyncTransforms();
    }

    private List<PathNode> GetEntryNodes(List<Neighbor> neighbors)
    {
        return neighbors.Select(n => {
            Vector3 offset = n.SharedSide switch
            {
                Side.Left => new Vector3(EntryOffset, 0f, 0f),
                Side.Right => new Vector3(-EntryOffset, 0f, 0f),
                Side.Forward => new Vector3(0f, 0f, -EntryOffset),
                Side.Back => new Vector3(0f, 0f, EntryOffset),
                _ => throw new System.ArgumentOutOfRangeException()
            };

            return new PathNode() { Point = n.PassagePoint + offset };
        }).ToList();
    }

    private List<PathNode> GetRandomInteriorNodes()
    {
        List<PathNode> interiorNode = new();

        int numExtraPoints = Random.Range(1, 4);
        float shrink = Floats.MinimumDoorWidth / 2f;
        for (int i = 0; i < numExtraPoints; i++)
        {
            Vector3 randomPoint = new(
                Random.Range(LeftBound + shrink, RightBound - shrink),
                Level,
                Random.Range(BackBound + shrink, ForwardBound - shrink)
            );

            PathNode newNode = new() { Point = randomPoint };
            interiorNode.Add(newNode);
        }

        return interiorNode;
    }

    private void CreateMinimumSpanningTree(List<PathNode> nodes)
    {
        if (nodes.Count == 0) return;

        PathNode startingNode = nodes[0];

        List<PathNode> untouchedNodes = new(nodes);
        untouchedNodes.Remove(startingNode);
        List<PathNode> connectedNodes = new() { startingNode };

        int breaker = 0;
        while (connectedNodes.Count < nodes.Count)
        {
            breaker++;
            if (breaker > nodes.Count)
            {
                throw new System.InvalidOperationException("Breaker limit exceeded in ConnectNodes");
            }

            float closestDistance = float.MaxValue;
            PathNode closestConnected = null;
            PathNode closestUntouched = null;
            foreach (PathNode connectedNode in connectedNodes)
            {
                foreach (PathNode untouchedNode in untouchedNodes)
                {
                    float distance = (connectedNode.Point - untouchedNode.Point).sqrMagnitude;

                    if (closestConnected == null || closestUntouched == null)
                    {
                        closestDistance = distance;
                        closestConnected = connectedNode;
                        closestUntouched = untouchedNode;
                        continue;
                    }

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestConnected = connectedNode;
                        closestUntouched = untouchedNode;
                    }
                }
            }

            closestConnected.Neighbors.Add(closestUntouched);
            closestUntouched.Neighbors.Add(closestConnected);
            connectedNodes.Add(closestUntouched);
            untouchedNodes.Remove(closestUntouched);
        }
    }

    private void ConnectEntryAndInterior(List<PathNode> entryNodes, List<PathNode> interiorNodes)
    {
        if (interiorNodes.Count > 0)
        {
            foreach (PathNode entryNode in entryNodes)
            {
                PathNode closestNode = interiorNodes.OrderBy(n => (n.Point - entryNode.Point).sqrMagnitude).First();
                entryNode.Neighbors.Add(closestNode);
                closestNode.Neighbors.Add(entryNode);
            }
        }
        else
        {
            CreateMinimumSpanningTree(entryNodes);
        }
    }

    private void InstantiatePaths(List<PathNode> nodes)
    {
        PathNode firstNode = nodes[0];
        List<PathNode> visitedNodes = new() { firstNode };
        Queue<PathNode> queue = new();
        queue.Enqueue(firstNode);

        while (queue.Count > 0)
        {
            PathNode node = queue.Dequeue();

            foreach (PathNode neighbour in node.Neighbors)
            {
                if (!visitedNodes.Contains(neighbour))
                {
                    InstantiatePath(node.Point, neighbour.Point);
                    visitedNodes.Add(neighbour);
                    queue.Enqueue(neighbour);
                }
            }
        }
    }

    private void InstantiatePath(Vector3 point1, Vector3 point2)
    {
        Vector3 midpoint = (point1 + point2) / 2f;
        float length = Vector3.Distance(point1, point2);
        Vector3 direction = (point1 - point2).normalized;

        Quaternion orientation = Quaternion.LookRotation(direction);
        Vector3 size = new Vector3(PathWidth, PathWidth, length);

        GameObject pathObject = Instantiate(PathPrefab, midpoint, orientation);
        pathObject.transform.localScale = size;
        pathObject.transform.SetParent(transform);
    }
}
