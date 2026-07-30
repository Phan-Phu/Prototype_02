using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Domain;
using Infrastructure;

namespace Application
{
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding Instance;

        [SerializeField] Transform gameObjectGridPrefab;
        [SerializeField] LayerMask layerMaskObstacle;

        private int witdth;
        private int height;
        private int cellSize;

        private IGridSystemHex<PathNode> gridSystem;
        private readonly IPathfindingAlgorithm aStarPathfinder = new AStarPathfinder();

        private void Awake()
        {
            if (Instance != null)
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
        (IGridSystemHex<PathNode> gridSystem, GridPosition gridPosition) => new PathNode(gridPosition));
            //gridSystem.GridDebugObject(gameObjectGridPrefab);

            for (int x = 0; x < witdth; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridPosition gridPosition = new GridPosition(x, y);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

                    float rayCastOffestDistance = 5f;
                    if (Physics.Raycast(worldPosition + Vector3.down * rayCastOffestDistance, Vector3.up, rayCastOffestDistance * 2, layerMaskObstacle))
                    {
                        GetNode(x, y).SetIsWalkable(false);
                    }
                }
            }
        }

        public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
        {
            return aStarPathfinder.FindPath(gridSystem, startGridPosition, endGridPosition, out pathLength);
        }

        private PathNode GetNode(int x, int y)
        {
            return gridSystem.GetGridObject(new GridPosition(x, y));
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
}
