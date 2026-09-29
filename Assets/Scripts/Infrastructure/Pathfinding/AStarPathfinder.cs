using System.Collections.Generic;
using UnityEngine;
using Domain;
using AStarPathfinding;

namespace Infrastructure
{
    // Adapter over the grid-agnostic AStarPathfinding library (Assets/Scripts/AStarPathfinding) -
    // bridges the game's hex GridPosition/PathNode/IGridSystemHex<PathNode> to the generic
    // search. Callers own the grid, walkability data and raycast/obstacle setup (see
    // Application/Pathfinding/Pathfinding.cs).
    public class AStarPathfinder : IPathfindingAlgorithm
    {
        private const int MOVE_STRAIGHT_COST = 10;

        public List<GridPosition> FindPath(IGridSystemHex<PathNode> gridSystem, GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
        {
            HexGridAdapter grid = new HexGridAdapter(gridSystem);
            return AStarSearch.FindPath(grid, startGridPosition, endGridPosition, out pathLength);
        }

        // Wraps the Domain hex grid so the generic AStarSearch never sees PathNode/IGridSystemHex.
        private class HexGridAdapter : IAStarGrid<GridPosition>
        {
            private readonly IGridSystemHex<PathNode> gridSystem;

            public HexGridAdapter(IGridSystemHex<PathNode> gridSystem)
            {
                this.gridSystem = gridSystem;
            }

            public bool IsWalkable(GridPosition position)
            {
                return gridSystem.GetGridObject(position).IsWalkable();
            }

            public int GetMoveCost(GridPosition from, GridPosition to)
            {
                return MOVE_STRAIGHT_COST;
            }

            public int GetHeuristicCost(GridPosition from, GridPosition to)
            {
                return MOVE_STRAIGHT_COST *
                    Mathf.RoundToInt(Vector3.Distance(gridSystem.GetWorldPosition(from), gridSystem.GetWorldPosition(to)));
            }

            public void OnNodeCostsUpdated(GridPosition position, int gCost, int hCost, int fCost)
            {
                PathNode pathNode = gridSystem.GetGridObject(position);
                pathNode.SetGCost(gCost);
                pathNode.SetHCost(hCost);
                pathNode.CaculateFCost();
            }

            public IEnumerable<GridPosition> GetNeighbours(GridPosition position)
            {
                List<GridPosition> neighbours = new List<GridPosition>();

                if (position.x - 1 >= 0)
                {
                    // Left
                    neighbours.Add(new GridPosition(position.x - 1, position.y));
                }

                if (position.x + 1 < gridSystem.GetWidth())
                {
                    // Right
                    neighbours.Add(new GridPosition(position.x + 1, position.y));
                }

                if (position.y + 1 < gridSystem.GetHeight())
                {
                    // Up
                    neighbours.Add(new GridPosition(position.x, position.y + 1));
                }

                if (position.y - 1 >= 0)
                {
                    // Down
                    neighbours.Add(new GridPosition(position.x, position.y - 1));
                }

                bool oddRow = position.y % 2 == 1;
                if (oddRow)
                {
                    if (position.x + 1 < gridSystem.GetWidth())
                    {
                        if (position.y - 1 >= 0)
                        {
                            neighbours.Add(new GridPosition(position.x + 1, position.y - 1));
                        }
                        if (position.y + 1 < gridSystem.GetHeight())
                        {
                            neighbours.Add(new GridPosition(position.x + 1, position.y + 1));
                        }
                    }
                }
                else
                {
                    if (position.x - 1 >= 0)
                    {
                        if (position.y - 1 >= 0)
                        {
                            neighbours.Add(new GridPosition(position.x - 1, position.y - 1));
                        }
                        if (position.y + 1 < gridSystem.GetHeight())
                        {
                            neighbours.Add(new GridPosition(position.x - 1, position.y + 1));
                        }
                    }
                }

                return neighbours;
            }
        }
    }
}
