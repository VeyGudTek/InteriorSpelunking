using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropPathGenerator : MonoBehaviour
{
    private const float EntryOffset = 0.25f;

    [System.Serializable]
    private class PathNode
    {
        public Vector3 Point;
        public List<PathNode> Neighbors = new();
    }

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
        List<PathNode> entryNodes = GetEntryNodes(neighbors);
        List<PathNode> interiorNode = GetRandomInteriorNodes();

        CreateMinimumSpanningTree(interiorNode);
        ConnectEntryAndInterior(entryNodes, interiorNode);

        List<PathNode> allNodes = entryNodes.Concat(interiorNode).ToList();
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
            Vector3 randomPoint = new Vector3(
                Random.Range(LeftBound + shrink, RightBound - shrink),
                Level,
                Random.Range(BackBound + shrink, ForwardBound - shrink)
            );

            PathNode newNode = new PathNode() { Point = randomPoint };
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
}
