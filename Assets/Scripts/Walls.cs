using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Walls : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject WallPrefab;

    [Header("Settings")]
    [SerializeField]
    private float ChanceToCreateDoorway = 0.5f;
    [SerializeField]
    private float DoorHeight = 2f;

    [Header("State")]
    public bool HasCreatedWalls = false;

    private float LeftBound;
    private float RightBound;
    private float ForwardBound;
    private float BackwardBound;
    private float Height;
    private float Level;
    private float CenterY => Level + (Height / 2f);

    private Dictionary<Side, List<(float, float)>> SolidWallsToCreate;

    public void CreateWalls(float leftBound, float rightBound, float forwardBound, float backwardBound, float height, float level, List<Neighbor> neighbors)
    {
        AssignProperties(leftBound, rightBound, forwardBound, backwardBound, height, level);
        
        foreach (Neighbor neighbor in neighbors)
        {
            if (neighbor.HasPassage && !neighbor.OtherRoom.Walls.HasCreatedWalls)
            {
                CreateDoorway(neighbor);
            }
            if (neighbor.HasPassage || neighbor.OtherRoom.Walls.HasCreatedWalls)
            {
                ClampSolidWalls(neighbor);
            }
        }

        CreateSolidWalls();

        HasCreatedWalls = true;
    }

    private void AssignProperties(float leftBound, float rightBound, float forwardBound, float backwardBound, float height, float level)
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
        Level = level;
    }

    private void CreateDoorway(Neighbor neighbor)
    {
        if (Random.value > ChanceToCreateDoorway)
        {
            return;
        }

        bool isHorizontal = neighbor.SharedSide.IsHorizontal();

        float edgeStart = isHorizontal ? BackwardBound : LeftBound;
        float edgeEnd = isHorizontal ? ForwardBound : RightBound;
        float neighborEdgeStart = isHorizontal ? neighbor.OtherRoom.BackwardBound : neighbor.OtherRoom.LeftBound;
        float neighborEdgeEnd = isHorizontal ? neighbor.OtherRoom.ForwardBound : neighbor.OtherRoom.RightBound;

        float wallStart = Mathf.Max(edgeStart, neighborEdgeStart);
        float wallEnd = Mathf.Min(edgeEnd, neighborEdgeEnd);

        CreateDoorTop(neighbor.SharedSide, wallStart, wallEnd);

        float randomDoorCenter = Random.Range(wallStart + (Floats.MinimumDoorWidth / 2f), wallEnd - (Floats.MinimumDoorWidth / 2f));
        float randomDoorWidth = Random.Range(Floats.MinimumDoorWidth, wallEnd - wallStart);

        CreateDoorSides(neighbor.SharedSide, randomDoorCenter, randomDoorWidth, wallStart, wallEnd);
    }

    private void CreateDoorTop(Side sharedSide, float wallStart, float wallEnd)
    {
        float doorTop = Level + DoorHeight;
        float topBound = Level + Height;

        float wallCenterY = (doorTop + topBound) / 2f;
        float wallHeight = topBound - doorTop;

        float wallCenter = (wallStart + wallEnd) / 2f;
        float wallLength = wallEnd - wallStart;

        Vector3 position = sharedSide switch
        {
            Side.Left => new Vector3(LeftBound, wallCenterY, wallCenter),
            Side.Right => new Vector3(RightBound, wallCenterY, wallCenter),
            Side.Forward => new Vector3(wallCenter, wallCenterY, ForwardBound),
            Side.Back => new Vector3(wallCenter, wallCenterY, BackwardBound),
            _ => throw new System.NotImplementedException()
        };
        Vector3 size = sharedSide switch
        {
            Side.Left => new Vector3(Floats.WallThickness, wallHeight, wallLength),
            Side.Right => new Vector3(Floats.WallThickness, wallHeight, wallLength),
            Side.Forward => new Vector3(wallLength, wallHeight, Floats.WallThickness),
            Side.Back => new Vector3(wallLength, wallHeight, Floats.WallThickness),
            _ => throw new System.NotImplementedException()
        };

        GameObject wall = Instantiate(WallPrefab, position, Quaternion.identity);
        wall.transform.localScale = size;
        wall.transform.SetParent(transform);
    }

    private void CreateDoorSides(Side sharedSide, float doorCenter, float doorWidth, float wallStart, float wallEnd)
    {
        float doorMin = doorCenter - (doorWidth / 2f);
        float doorMax = doorCenter + (doorWidth / 2f);

        float wallCenterY = Level + (DoorHeight / 2f);

        if (doorMin > wallStart)
        {
            float leftWallCenter = (wallStart + doorMin) / 2f;
            Vector3 position = sharedSide switch
            {
                Side.Left => new Vector3(LeftBound, wallCenterY, leftWallCenter),
                Side.Right => new Vector3(RightBound, wallCenterY, leftWallCenter),
                Side.Forward => new Vector3(leftWallCenter, wallCenterY, ForwardBound),
                Side.Back => new Vector3(leftWallCenter, wallCenterY, BackwardBound),
                _ => throw new System.NotImplementedException()
            };
            Vector3 size = sharedSide switch
            {
                Side.Left => new Vector3(Floats.WallThickness, DoorHeight, doorMin - wallStart),
                Side.Right => new Vector3(Floats.WallThickness, DoorHeight, doorMin - wallStart),
                Side.Forward => new Vector3(doorMin - wallStart, DoorHeight, Floats.WallThickness),
                Side.Back => new Vector3(doorMin - wallStart, DoorHeight, Floats.WallThickness),
                _ => throw new System.NotImplementedException()
            };

            GameObject wall = Instantiate(WallPrefab, position, Quaternion.identity);
            wall.transform.localScale = size;
            wall.transform.SetParent(transform);
        }
        if (doorMax < wallEnd)
        {
            float rightWallCenter = (wallEnd + doorMax) / 2f;
            Vector3 position = sharedSide switch
            {
                Side.Left => new Vector3(LeftBound, wallCenterY, rightWallCenter),
                Side.Right => new Vector3(RightBound, wallCenterY, rightWallCenter),
                Side.Forward => new Vector3(rightWallCenter, wallCenterY, ForwardBound),
                Side.Back => new Vector3(rightWallCenter, wallCenterY, BackwardBound),
                _ => throw new System.NotImplementedException()
            };
            Vector3 size = sharedSide switch
            {
                Side.Left => new Vector3(Floats.WallThickness, DoorHeight, wallEnd - doorMax),
                Side.Right => new Vector3(Floats.WallThickness, DoorHeight, wallEnd - doorMax),
                Side.Forward => new Vector3(wallEnd - doorMax, DoorHeight, Floats.WallThickness),
                Side.Back => new Vector3(wallEnd - doorMax, DoorHeight, Floats.WallThickness),
                _ => throw new System.NotImplementedException()
            };

            GameObject wall = Instantiate(WallPrefab, position, Quaternion.identity);
            wall.transform.localScale = size;
            wall.transform.SetParent(transform);
        }
    }

    private void ClampSolidWalls(Neighbor neighbor)
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
