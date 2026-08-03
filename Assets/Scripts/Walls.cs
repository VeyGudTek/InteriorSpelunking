using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Walls : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject WallPrefab;

    private float LeftBound;
    private float RightBound;
    private float ForwardBound;
    private float BackwardBound;
    private float Height;
    private float CenterY;
    private Dictionary<Side, List<(float, float)>> SolidWallsToCreate;

    public void CreateWalls(float leftBound, float rightBound, float forwardBound, float backwardBound, float height, float centerY, List<Neighbor> neighbors)
    {
        AssignProperties(leftBound, rightBound, forwardBound, backwardBound, height, centerY);
        
        foreach (Neighbor neighbor in neighbors.Where(n => n.HasPassage))
        {
            CreateDoorway();
            PopulateSolidWalls(neighbor);
        }

        CreateSolidWalls();
    }

    private void AssignProperties(float leftBound, float rightBound, float forwardBound, float backwardBound, float height, float centerY)
    {
        SolidWallsToCreate = new()
        {
            { Side.Left, new(){ (backwardBound, forwardBound) } },
            { Side.Right, new(){ (backwardBound, forwardBound) } },
            { Side.Forward, new(){ (leftBound, rightBound) } },
            { Side.Back, new(){ (leftBound, rightBound) } }
        };

        LeftBound = leftBound; 
        RightBound = rightBound;
        ForwardBound = forwardBound;
        BackwardBound = backwardBound;
        Height = height;
        CenterY = centerY;
    }

    private void CreateDoorway()
    {
        //Skip this for now
    }

    private void PopulateSolidWalls(Neighbor neighbor)
    {
        Side sideToClamp = neighbor.SharedSide;
        List<(float, float)> existingAvailableEdges = SolidWallsToCreate[sideToClamp];
        List<(float, float)> newAvailableEdges = new();

        bool isHorizontal = sideToClamp == Side.Left || sideToClamp == Side.Right;
        float neighborStart = isHorizontal ? neighbor.OtherRoom.BackwardBound : neighbor.OtherRoom.LeftBound;
        float neighborEnd = isHorizontal ? neighbor.OtherRoom.ForwardBound : neighbor.OtherRoom.RightBound;

        foreach ((float start, float end) in existingAvailableEdges)
        {
            if (neighborEnd <= start || neighborStart >= end)
            {
                newAvailableEdges.Add((start, end));
                continue;
            }
            
            if (neighborStart > start && neighborEnd < end)
            {
                newAvailableEdges.Add((start, neighborStart));
                newAvailableEdges.Add((neighborEnd, end));
                continue;
            }

            if (neighborStart > start)
            {
                newAvailableEdges.Add((start, neighborStart));
                continue;
            }

            if (neighborEnd < end)
            {
                newAvailableEdges.Add((neighborEnd, end)); 
                continue;    
            }

            if (neighborStart <= start && neighborEnd >= end)
            {
                continue;
            }

            throw new System.InvalidOperationException("Error recalculating available edges.");
        }

        SolidWallsToCreate[sideToClamp] = newAvailableEdges.Where(e=> e.Item2 - e.Item1 > Floats.EqualityThreshold).ToList();
    }

    private void CreateSolidWalls()
    {
        foreach ((Side side, List<(float, float)> edges) in SolidWallsToCreate)
        {
            foreach ((float start, float end) in edges)
            {
                Vector3 position = side switch
                {
                    Side.Left => new Vector3(LeftBound, CenterY, (end + start) / 2f),
                    Side.Right => new Vector3(RightBound, CenterY, (end + start) / 2f),
                    Side.Forward => new Vector3((end + start) / 2f, CenterY, ForwardBound),
                    Side.Back => new Vector3((end + start) / 2f, CenterY, BackwardBound),
                    _ => throw new System.NotImplementedException()
                };
                Vector3 size = side switch
                {
                    Side.Left => new Vector3(Floats.WallThickness, Height, end - start),
                    Side.Right => new Vector3(Floats.WallThickness, Height, end - start),
                    Side.Forward => new Vector3(end - start, Height, Floats.WallThickness),
                    Side.Back => new Vector3(end - start, Height, Floats.WallThickness),
                    _ => throw new System.NotImplementedException()
                };

                GameObject wall = Instantiate(WallPrefab, position, Quaternion.identity);
                wall.transform.localScale = size;
                wall.transform.SetParent(transform);
            }
        }
    }
}
