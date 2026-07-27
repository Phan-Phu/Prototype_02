using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;

    private const int MOVE_STRAIGHT_COST = 10;

    [SerializeField] Transform gameObjectGridPrefab;
    [SerializeField] LayerMask layerMaskObstacle; 

    private int witdth;
    private int height;
    private int cellSize;

    private GridSystemHex<PathNode> gridSystem;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Has more one than Pathfinding: " + transform + ", " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Setup(int witdth, int height, int cellSize)
    {
        this.witdth = witdth;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystemHex<PathNode>(witdth, height, cellSize,
    (GridSystemHex<PathNode> gridSystem, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.GridDebugObject(gameObjectGridPrefab);

        for (int x = 0; x < witdth; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

                float rayCastOffestDistance = 5f;
                if(Physics.Raycast(worldPosition + Vector3.down * rayCastOffestDistance, Vector3.up, rayCastOffestDistance * 2, layerMaskObstacle))
                {
                    GetNode(x, y).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int y = 0; y < gridSystem.GetHeight(); y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CaculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateHeuristicDistance(startGridPosition, endGridPosition));
        startNode.CaculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neigbourNode in GetNeighbourList(currentNode))
            {
                if(closedList.Contains(neigbourNode))
                {
                    continue;
                }
                if(!neigbourNode.IsWalkable())
                {
                    closedList.Add(neigbourNode);
                    continue;
                }

                int tentativeGCost = 
                    currentNode.GetGCost() + MOVE_STRAIGHT_COST;
                if(tentativeGCost < neigbourNode.GetGCost())
                {
                    neigbourNode.SetCameFromPathNode(currentNode);
                    neigbourNode.SetGCost(tentativeGCost);
                    neigbourNode.SetHCost(CalculateHeuristicDistance(neigbourNode.GetGridPostion(), endGridPosition));
                    neigbourNode.CaculateFCost();

                    if(!openList.Contains(neigbourNode))
                    {
                        openList.Add(neigbourNode);
                    }
                }
            }
        }

        // no Path found
        pathLength = 0;
        return null;
    }

    private int CalculateHeuristicDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        return MOVE_STRAIGHT_COST *
            Mathf.RoundToInt(Vector3.Distance(gridSystem.GetWorldPosition(gridPositionA), gridSystem.GetWorldPosition(gridPositionB)));
    }
    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode pathNodeLowestFCost = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].GetFCost() < pathNodeLowestFCost.GetFCost())
            {
                pathNodeLowestFCost = pathNodeList[i];
            }
        }
        return pathNodeLowestFCost;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neigbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPostion();

        if (gridPosition.x - 1 >= 0)
        {
            //Left
            neigbourList.Add(GetNode(gridPosition.x - 1, gridPosition.y));
        }

        if (gridPosition.x + 1 < gridSystem.GetWidth())
        {
            //Right
            neigbourList.Add(GetNode(gridPosition.x + 1, gridPosition.y));
        }

        if (gridPosition.y + 1 < gridSystem.GetHeight())
        {
            //Up
            neigbourList.Add(GetNode(gridPosition.x, gridPosition.y + 1));
        }

        if (gridPosition.y - 1 >= 0)
        {
            //Down
            neigbourList.Add(GetNode(gridPosition.x, gridPosition.y - 1));
        }

        bool oddRow = gridPosition.y % 2 == 1;
        if(oddRow)
        {
            if (gridPosition.x + 1 < gridSystem.GetWidth())
            {
                if (gridPosition.y - 1 >= 0)
                {
                    neigbourList.Add(GetNode(gridPosition.x + 1, gridPosition.y - 1));
                }
                if (gridPosition.y + 1 <= gridSystem.GetHeight())
                {
                    neigbourList.Add(GetNode(gridPosition.x + 1, gridPosition.y + 1));
                }
            }

        }
        else
        {
            if (gridPosition.x - 1 >= 0)
            {
                if (gridPosition.y - 1 >= 0)
                {
                    neigbourList.Add(GetNode(gridPosition.x - 1, gridPosition.y - 1));
                }
                if (gridPosition.y + 1 <= gridSystem.GetHeight())
                {
                    neigbourList.Add(GetNode(gridPosition.x - 1, gridPosition.y + 1));
                }
            }
        }


        return neigbourList;
    }

    private PathNode GetNode(int x, int y)
    {
        return gridSystem.GetGridObject(new GridPosition(x, y));
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();

        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;

        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPostion());
        }

        return gridPositionList;
    }

    public void SetIsWalkableGridPositon(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool IsWalkableGridPositon(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
