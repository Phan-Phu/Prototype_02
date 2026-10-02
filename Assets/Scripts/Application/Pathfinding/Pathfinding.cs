using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Domain;
using AStarPathfinding;

namespace Application
{
    // Owns the level's walkability map and answers path queries with the standalone
    // AStarPathfinding library. Hex topology comes from LevelGrid.
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding Instance;

        private const int MOVE_STRAIGHT_COST = 10;

        [SerializeField] Transform gameObjectGridPrefab;
        [SerializeField] LayerMask layerMaskObstacle;

        private int witdth;
        private int height;
        private int cellSize;

        private bool[,] isWalkableArray;
        private HexGridAdapter hexGridAdapter;

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

            isWalkableArray = new bool[witdth, height];
            hexGridAdapter = new HexGridAdapter(this);

            for (int x = 0; x < witdth; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridPosition gridPosition = new GridPosition(x, y);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

                    float rayCastOffestDistance = 5f;
                    bool isBlocked = Physics.Raycast(worldPosition + Vector3.down * rayCastOffestDistance, Vector3.up, rayCastOffestDistance * 2, layerMaskObstacle);
                    isWalkableArray[x, y] = !isBlocked;
                }
            }
        }

        public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
        {
            return AStarSearch.FindPath(hexGridAdapter, startGridPosition, endGridPosition, out pathLength);
        }

        public void SetIsWalkableGridPositon(GridPosition gridPosition, bool isWalkable)
        {
            if (!LevelGrid.Instance.IsValidGridPosition(gridPosition))
            {
                Debug.LogError($"Cannot set walkability outside the level grid at {gridPosition}.");
                return;
            }
            isWalkableArray[gridPosition.x, gridPosition.y] = isWalkable;
        }

        // Cells outside the grid are never walkable.
        public bool IsWalkableGridPositon(GridPosition gridPosition)
        {
            return LevelGrid.Instance.IsValidGridPosition(gridPosition) && isWalkableArray[gridPosition.x, gridPosition.y];
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

        // Bridges the game's hex grid to the grid-agnostic A* library.
        private class HexGridAdapter : IAStarGrid<GridPosition>
        {
            private readonly Pathfinding pathfinding;

            public HexGridAdapter(Pathfinding pathfinding)
            {
                this.pathfinding = pathfinding;
            }

            public bool IsWalkable(GridPosition position)
            {
                return pathfinding.IsWalkableGridPositon(position);
            }

            public IEnumerable<GridPosition> GetNeighbours(GridPosition position)
            {
                return LevelGrid.Instance.GetNeighbours(position);
            }

            public int GetMoveCost(GridPosition from, GridPosition to)
            {
                return MOVE_STRAIGHT_COST;
            }

            public int GetHeuristicCost(GridPosition from, GridPosition to)
            {
                // Hex step count never overestimates the real cost, so A* still finds the shortest path.
                return MOVE_STRAIGHT_COST * LevelGrid.Instance.GetDistance(from, to);
            }
        }
    }
}
